import {Component, Event, EventEmitter, h, Host, Listen, Method, Prop, State, Watch} from '@stencil/core';
import {eventBus} from '../../../../services/event-bus';
import {ActivityDefinition, ActivityDescriptor, ActivityModel, ConnectionDefinition, ConnectionModel, EventTypes, WorkflowDefinition, WorkflowModel, WorkflowPersistenceBehavior} from "../../../../models";
import {createElsaClient, SaveWorkflowDefinitionRequest} from "../../../../services/elsa-client";
import {pluginManager} from '../../../../services/plugin-manager';
import state from '../../../../utils/store';
import Tunnel, {WorkflowEditorState} from '../../../data/workflow-editor';

@Component({
  tag: 'elsa-workflow-editor',
  styleUrl: 'elsa-workflow-editor.css',
  shadow: false,
})
export class ElsaWorkflowDefinitionEditor {

  constructor() {
    pluginManager.initialize();
  }

  @Prop({attribute: 'workflow-definition-id', reflect: true}) workflowDefinitionId: string;
  @Prop({attribute: 'server-url', reflect: true}) serverUrl: string;
  @Prop({attribute: 'monaco-lib-path', reflect: true}) monacoLibPath: string;
  @State() workflowDefinition: WorkflowDefinition;
  @State() workflowModel: WorkflowModel;
  @State() publishing: boolean;
  @State() unPublishing: boolean;
  @State() unPublished: boolean;
  @State() saving: boolean;
  @State() saved: boolean;
  @State() networkError: string;
  el: HTMLElement;
  designer: HTMLElsaDesignerTreeElement;

  @Method()
  async getServerUrl(): Promise<string> {
    return this.serverUrl;
  }

  @Method()
  async getWorkflowDefinitionId(): Promise<string> {
    return this.workflowDefinition.definitionId;
  }

  @Watch('workflowDefinitionId')
  async workflowDefinitionIdChangedHandler(newValue: string) {
    const workflowDefinitionId = newValue;
    let workflowDefinition: WorkflowDefinition = this.createWorkflowDefinition();
    const client = createElsaClient(this.serverUrl);

    if (workflowDefinitionId && workflowDefinitionId.length > 0) {
      try {
        workflowDefinition = await client.workflowDefinitionsApi.getByDefinitionAndVersion(workflowDefinitionId, {isLatest: true});
      } catch {
        console.warn(`The specified workflow definition does not exist. Creating a new one.`)
      }
    }

    this.updateWorkflowDefinition(workflowDefinition);
  }

  @Watch("serverUrl")
  async serverUrlChangedHandler(newValue: string) {
    if (newValue && newValue.length > 0)
      await this.loadActivityDescriptors();
  }

  @Watch("monacoLibPath")
  async monacoLibPathChangedHandler(newValue: string) {
    state.monacoLibPath = newValue;
  }

  @Listen('workflow-changed')
  async workflowChangedHandler(event: CustomEvent<WorkflowModel>) {
    const workflowModel = event.detail;
    await this.saveWorkflowInternal(workflowModel);
  }

  async componentWillLoad() {
    await this.serverUrlChangedHandler(this.serverUrl);
    await this.workflowDefinitionIdChangedHandler(this.workflowDefinitionId);
    await this.monacoLibPathChangedHandler(this.monacoLibPath);
  }

  componentDidLoad() {
    if (!this.designer) {
      this.designer = this.el.querySelector("elsa-designer-tree") as HTMLElsaDesignerTreeElement;
      this.designer.model = this.workflowModel;
    }

    eventBus.on(EventTypes.UpdateWorkflowSettings, async (workflowDefinition: WorkflowDefinition) => {
      this.updateWorkflowDefinition(workflowDefinition);
      await this.saveWorkflowInternal(this.workflowModel);
    });
  }

  async loadActivityDescriptors() {
    const client = createElsaClient(this.serverUrl);
    state.activityDescriptors = await client.activitiesApi.list();
  }

  updateWorkflowDefinition(value: WorkflowDefinition) {
    this.workflowDefinition = value;
    this.workflowModel = this.mapWorkflowModel(value);
  }

  async publishWorkflow() {
    this.publishing = true;
    await this.saveWorkflow(true);
    this.publishing = false;
    eventBus.emit(EventTypes.WorkflowPublished, this, this.workflowDefinition);
  }

  async unPublishWorkflow() {
    this.unPublishing = true;
    await this.unpublishWorkflow();
    this.unPublishing = false;
    eventBus.emit(EventTypes.WorkflowRetracted, this, this.workflowDefinition);
  }

  async saveWorkflow(publish?: boolean) {
    await this.saveWorkflowInternal(null, publish);
  }

  async saveWorkflowInternal(workflowModel?: WorkflowModel, publish?: boolean) {
    if (!this.serverUrl || this.serverUrl.length == 0)
      return;

    workflowModel = workflowModel || this.workflowModel;

    const client = createElsaClient(this.serverUrl);
    let workflowDefinition = this.workflowDefinition;

    const request: SaveWorkflowDefinitionRequest = {
      workflowDefinitionId: workflowDefinition.definitionId || this.workflowDefinitionId,
      contextOptions: workflowDefinition.contextOptions,
      deleteCompletedInstances: workflowDefinition.deleteCompletedInstances,
      description: workflowDefinition.description,
      displayName: workflowDefinition.displayName,
      isSingleton: workflowDefinition.isSingleton,
      name: workflowDefinition.name,
      persistenceBehavior: workflowDefinition.persistenceBehavior,
      publish: publish || false,
      variables: workflowDefinition.variables,
      activities: workflowModel.activities.map<ActivityDefinition>(x => ({
        activityId: x.activityId,
        type: x.type,
        name: x.name,
        displayName: x.displayName,
        description: x.description,
        persistWorkflow: x.persistWorkflow,
        loadWorkflowContext: x.loadWorkflowContext,
        saveWorkflowContext: x.saveWorkflowContext,
        persistOutput: x.persistOutput,
        properties: x.properties
      })),
      connections: workflowModel.connections.map<ConnectionDefinition>(x => ({
        sourceActivityId: x.sourceId,
        targetActivityId: x.targetId,
        outcome: x.outcome
      })),
    };

    this.saving = !publish;
    this.publishing = publish;

    try {
      workflowDefinition = await client.workflowDefinitionsApi.save(request);
      this.workflowDefinition = workflowDefinition;
      this.workflowModel = this.mapWorkflowModel(workflowDefinition);

      this.saving = false;
      this.saved = !publish;
      this.publishing = false;
      setTimeout(() => this.saved = false, 500);
    } catch (e) {
      console.error(e);
      this.saving = false;
      this.saved = false;
      this.networkError = e.message;
      setTimeout(() => this.networkError = null, 2000);
    }
  }

  async unpublishWorkflow() {
    const client = createElsaClient(this.serverUrl);
    const workflowDefinitionId = this.workflowDefinition.definitionId;
    this.unPublishing = true;

    try {
      this.workflowDefinition = await client.workflowDefinitionsApi.retract(workflowDefinitionId);
      this.unPublishing = false;
      this.unPublished = true
      setTimeout(() => this.unPublished = false, 500);
    } catch (e) {
      console.error(e);
      this.unPublishing = false;
      this.unPublished = false;
      this.networkError = e.message;
      setTimeout(() => this.networkError = null, 2000);
    }
  }

  mapWorkflowModel(workflowDefinition: WorkflowDefinition): WorkflowModel {
    return {
      activities: workflowDefinition.activities.map(this.mapActivityModel),
      connections: workflowDefinition.connections.map(this.mapConnectionModel)
    };
  }

  mapActivityModel(source: ActivityDefinition): ActivityModel {
    const descriptors: Array<ActivityDescriptor> = state.activityDescriptors;
    const descriptor = descriptors.find(x => x.type == source.type);

    return {
      activityId: source.activityId,
      description: source.description,
      displayName: source.displayName,
      name: source.name,
      type: source.type,
      properties: source.properties,
      outcomes: [...descriptor.outcomes],
      persistOutput: source.persistOutput,
      persistWorkflow: source.persistWorkflow,
      saveWorkflowContext: source.saveWorkflowContext,
      loadWorkflowContext: source.loadWorkflowContext
    }
  }

  mapConnectionModel(source: ConnectionDefinition): ConnectionModel {
    return {
      sourceId: source.sourceActivityId,
      targetId: source.targetActivityId,
      outcome: source.outcome
    }
  }

  onShowWorkflowSettingsClick() {
    eventBus.emit(EventTypes.ShowWorkflowSettings);
  }

  async onPublishClicked() {
    await this.publishWorkflow();
  }

  async onUnPublishClicked() {
    await this.unPublishWorkflow();
  }

  render() {
    const tunnelState: WorkflowEditorState = {
      serverUrl: this.serverUrl,
      workflowDefinitionId: this.workflowDefinition.definitionId
    };

    return (
      <Host class="flex flex-col w-full" ref={el => this.el = el}>
        <Tunnel.Provider state={tunnelState}>
          {this.renderCanvas()}
          {this.renderActivityPicker()}
          {this.renderActivityEditor()}
        </Tunnel.Provider>
      </Host>
    );
  }

  renderCanvas() {
    return (
      <div class="h-screen flex relative">
        <elsa-designer-tree model={this.workflowModel} class="flex-1" ref={el => this.designer = el}/>
        {this.renderWorkflowSettingsButton()}
        <elsa-workflow-settings-modal workflowDefinition={this.workflowDefinition}/>
        <elsa-workflow-editor-notifications/>
        <div class="fixed bottom-10 right-12">
          <div class="flex items-center space-x-4">
            {this.renderSavingIndicator()}
            {this.renderNetworkError()}
            {this.renderPublishButton()}
          </div>
        </div>
      </div>
    );
  }

  renderActivityPicker() {
    return <elsa-activity-picker-modal/>;
  }

  renderActivityEditor() {
    return <elsa-activity-editor-modal/>;
  }

  renderWorkflowSettingsButton() {
    return (
      <button onClick={() => this.onShowWorkflowSettingsClick()} type="button"
              class="workflow-settings-button fixed top-10 right-12 inline-flex items-center p-2 rounded-full border border-transparent bg-white shadow text-gray-400 hover:text-blue-500 focus:text-blue-500 hover:ring-2 hover:ring-offset-2 hover:ring-blue-500 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" stroke="currentColor" fill="none" class="h-8 w-8">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z"/>
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"/>
        </svg>
      </button>
    );
  }

  renderWorkflowSettingsModal() {
    return;
  }

  renderSavingIndicator() {

    if (this.publishing)
      return undefined;

    const message =
      this.unPublishing ? 'Unpublishing...' : this.unPublished ? 'Unpublished'
        : this.saving ? 'Saving...' : this.saved ? 'Saved'
          : null;

    if (!message)
      return undefined;

    return (
      <div>
        <span class="text-gray-400 text-sm">{message}</span>
      </div>
    );
  }

  renderNetworkError() {
    if (!this.networkError)
      return undefined;

    return (
      <div>
        <span class="text-rose-400 text-sm">An error occurred: {this.networkError}</span>
      </div>);
  }

  renderPublishButton() {
    return <elsa-workflow-publish-button
      publishing={this.publishing}
      workflowDefinition={this.workflowDefinition}
      onPublishClicked={() => this.onPublishClicked()}
      onUnPublishClicked={() => this.onUnPublishClicked()}
    />;
  }

  private createWorkflowDefinition() : WorkflowDefinition {
    return {
      definitionId: this.workflowDefinitionId,
      version: 1,
      activities: [],
      connections: [],
      persistenceBehavior: WorkflowPersistenceBehavior.WorkflowBurst,
    };
  }
}
