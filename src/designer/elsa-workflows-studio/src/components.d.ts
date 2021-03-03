/* eslint-disable */
/* tslint:disable */
/**
 * This is an autogenerated file created by the Stencil compiler.
 * It contains typing information for all components that exist in this project.
 */
import { HTMLStencilElement, JSXBase } from "@stencil/core/internal";
import { ActivityDefinitionProperty, ActivityModel, ActivityPropertyDescriptor, WorkflowDefinition, WorkflowModel } from "./models";
import { MonacoValueChangedArgs } from "./components/editors/monaco/elsa-monaco/elsa-monaco";
export namespace Components {
    interface ElsaActivityEditorModal {
    }
    interface ElsaActivityPickerModal {
    }
    interface ElsaCheckListProperty {
        "propertyDescriptor": ActivityPropertyDescriptor;
        "propertyModel": ActivityDefinitionProperty;
        "serverUrl": string;
        "workflowDefinitionId": string;
    }
    interface ElsaDesignerTree {
        "model": WorkflowModel;
    }
    interface ElsaDesignerTreeActivity {
        "activityModel": ActivityModel;
        "icon": string;
    }
    interface ElsaModalDialog {
        "hide": (animate: boolean) => Promise<void>;
        "show": (animate: boolean) => Promise<void>;
    }
    interface ElsaMonaco {
        "addJavaScriptLib": (libSource: string, libUri: string) => Promise<void>;
        "editorHeight": string;
        "language": string;
        "singleLineMode": boolean;
        "value": string;
    }
    interface ElsaTextProperty {
        "editorHeight": string;
        "propertyDescriptor": ActivityPropertyDescriptor;
        "propertyModel": ActivityDefinitionProperty;
        "serverUrl": string;
        "singleLineMode": boolean;
        "workflowDefinitionId": string;
    }
    interface ElsaWorkflowEditor {
        "getServerUrl": () => Promise<string>;
        "getWorkflowDefinitionId": () => Promise<string>;
        "serverUrl": string;
        "workflowDefinitionId": string;
    }
    interface ElsaWorkflowPublishButton {
    }
    interface ElsaWorkflowSettingsModal {
        "workflowDefinition": WorkflowDefinition;
    }
}
declare global {
    interface HTMLElsaActivityEditorModalElement extends Components.ElsaActivityEditorModal, HTMLStencilElement {
    }
    var HTMLElsaActivityEditorModalElement: {
        prototype: HTMLElsaActivityEditorModalElement;
        new (): HTMLElsaActivityEditorModalElement;
    };
    interface HTMLElsaActivityPickerModalElement extends Components.ElsaActivityPickerModal, HTMLStencilElement {
    }
    var HTMLElsaActivityPickerModalElement: {
        prototype: HTMLElsaActivityPickerModalElement;
        new (): HTMLElsaActivityPickerModalElement;
    };
    interface HTMLElsaCheckListPropertyElement extends Components.ElsaCheckListProperty, HTMLStencilElement {
    }
    var HTMLElsaCheckListPropertyElement: {
        prototype: HTMLElsaCheckListPropertyElement;
        new (): HTMLElsaCheckListPropertyElement;
    };
    interface HTMLElsaDesignerTreeElement extends Components.ElsaDesignerTree, HTMLStencilElement {
    }
    var HTMLElsaDesignerTreeElement: {
        prototype: HTMLElsaDesignerTreeElement;
        new (): HTMLElsaDesignerTreeElement;
    };
    interface HTMLElsaDesignerTreeActivityElement extends Components.ElsaDesignerTreeActivity, HTMLStencilElement {
    }
    var HTMLElsaDesignerTreeActivityElement: {
        prototype: HTMLElsaDesignerTreeActivityElement;
        new (): HTMLElsaDesignerTreeActivityElement;
    };
    interface HTMLElsaModalDialogElement extends Components.ElsaModalDialog, HTMLStencilElement {
    }
    var HTMLElsaModalDialogElement: {
        prototype: HTMLElsaModalDialogElement;
        new (): HTMLElsaModalDialogElement;
    };
    interface HTMLElsaMonacoElement extends Components.ElsaMonaco, HTMLStencilElement {
    }
    var HTMLElsaMonacoElement: {
        prototype: HTMLElsaMonacoElement;
        new (): HTMLElsaMonacoElement;
    };
    interface HTMLElsaTextPropertyElement extends Components.ElsaTextProperty, HTMLStencilElement {
    }
    var HTMLElsaTextPropertyElement: {
        prototype: HTMLElsaTextPropertyElement;
        new (): HTMLElsaTextPropertyElement;
    };
    interface HTMLElsaWorkflowEditorElement extends Components.ElsaWorkflowEditor, HTMLStencilElement {
    }
    var HTMLElsaWorkflowEditorElement: {
        prototype: HTMLElsaWorkflowEditorElement;
        new (): HTMLElsaWorkflowEditorElement;
    };
    interface HTMLElsaWorkflowPublishButtonElement extends Components.ElsaWorkflowPublishButton, HTMLStencilElement {
    }
    var HTMLElsaWorkflowPublishButtonElement: {
        prototype: HTMLElsaWorkflowPublishButtonElement;
        new (): HTMLElsaWorkflowPublishButtonElement;
    };
    interface HTMLElsaWorkflowSettingsModalElement extends Components.ElsaWorkflowSettingsModal, HTMLStencilElement {
    }
    var HTMLElsaWorkflowSettingsModalElement: {
        prototype: HTMLElsaWorkflowSettingsModalElement;
        new (): HTMLElsaWorkflowSettingsModalElement;
    };
    interface HTMLElementTagNameMap {
        "elsa-activity-editor-modal": HTMLElsaActivityEditorModalElement;
        "elsa-activity-picker-modal": HTMLElsaActivityPickerModalElement;
        "elsa-check-list-property": HTMLElsaCheckListPropertyElement;
        "elsa-designer-tree": HTMLElsaDesignerTreeElement;
        "elsa-designer-tree-activity": HTMLElsaDesignerTreeActivityElement;
        "elsa-modal-dialog": HTMLElsaModalDialogElement;
        "elsa-monaco": HTMLElsaMonacoElement;
        "elsa-text-property": HTMLElsaTextPropertyElement;
        "elsa-workflow-editor": HTMLElsaWorkflowEditorElement;
        "elsa-workflow-publish-button": HTMLElsaWorkflowPublishButtonElement;
        "elsa-workflow-settings-modal": HTMLElsaWorkflowSettingsModalElement;
    }
}
declare namespace LocalJSX {
    interface ElsaActivityEditorModal {
    }
    interface ElsaActivityPickerModal {
    }
    interface ElsaCheckListProperty {
        "propertyDescriptor"?: ActivityPropertyDescriptor;
        "propertyModel"?: ActivityDefinitionProperty;
        "serverUrl"?: string;
        "workflowDefinitionId"?: string;
    }
    interface ElsaDesignerTree {
        "model"?: WorkflowModel;
        "onWorkflow-changed"?: (event: CustomEvent<WorkflowModel>) => void;
    }
    interface ElsaDesignerTreeActivity {
        "activityModel"?: ActivityModel;
        "icon"?: string;
        "onEdit-activity"?: (event: CustomEvent<ActivityModel>) => void;
        "onRemove-activity"?: (event: CustomEvent<ActivityModel>) => void;
    }
    interface ElsaModalDialog {
    }
    interface ElsaMonaco {
        "editorHeight"?: string;
        "language"?: string;
        "onValueChanged"?: (event: CustomEvent<MonacoValueChangedArgs>) => void;
        "singleLineMode"?: boolean;
        "value"?: string;
    }
    interface ElsaTextProperty {
        "editorHeight"?: string;
        "propertyDescriptor"?: ActivityPropertyDescriptor;
        "propertyModel"?: ActivityDefinitionProperty;
        "serverUrl"?: string;
        "singleLineMode"?: boolean;
        "workflowDefinitionId"?: string;
    }
    interface ElsaWorkflowEditor {
        "serverUrl"?: string;
        "workflowDefinitionId"?: string;
    }
    interface ElsaWorkflowPublishButton {
        "onPublishClicked"?: (event: CustomEvent<any>) => void;
    }
    interface ElsaWorkflowSettingsModal {
        "workflowDefinition"?: WorkflowDefinition;
    }
    interface IntrinsicElements {
        "elsa-activity-editor-modal": ElsaActivityEditorModal;
        "elsa-activity-picker-modal": ElsaActivityPickerModal;
        "elsa-check-list-property": ElsaCheckListProperty;
        "elsa-designer-tree": ElsaDesignerTree;
        "elsa-designer-tree-activity": ElsaDesignerTreeActivity;
        "elsa-modal-dialog": ElsaModalDialog;
        "elsa-monaco": ElsaMonaco;
        "elsa-text-property": ElsaTextProperty;
        "elsa-workflow-editor": ElsaWorkflowEditor;
        "elsa-workflow-publish-button": ElsaWorkflowPublishButton;
        "elsa-workflow-settings-modal": ElsaWorkflowSettingsModal;
    }
}
export { LocalJSX as JSX };
declare module "@stencil/core" {
    export namespace JSX {
        interface IntrinsicElements {
            "elsa-activity-editor-modal": LocalJSX.ElsaActivityEditorModal & JSXBase.HTMLAttributes<HTMLElsaActivityEditorModalElement>;
            "elsa-activity-picker-modal": LocalJSX.ElsaActivityPickerModal & JSXBase.HTMLAttributes<HTMLElsaActivityPickerModalElement>;
            "elsa-check-list-property": LocalJSX.ElsaCheckListProperty & JSXBase.HTMLAttributes<HTMLElsaCheckListPropertyElement>;
            "elsa-designer-tree": LocalJSX.ElsaDesignerTree & JSXBase.HTMLAttributes<HTMLElsaDesignerTreeElement>;
            "elsa-designer-tree-activity": LocalJSX.ElsaDesignerTreeActivity & JSXBase.HTMLAttributes<HTMLElsaDesignerTreeActivityElement>;
            "elsa-modal-dialog": LocalJSX.ElsaModalDialog & JSXBase.HTMLAttributes<HTMLElsaModalDialogElement>;
            "elsa-monaco": LocalJSX.ElsaMonaco & JSXBase.HTMLAttributes<HTMLElsaMonacoElement>;
            "elsa-text-property": LocalJSX.ElsaTextProperty & JSXBase.HTMLAttributes<HTMLElsaTextPropertyElement>;
            "elsa-workflow-editor": LocalJSX.ElsaWorkflowEditor & JSXBase.HTMLAttributes<HTMLElsaWorkflowEditorElement>;
            "elsa-workflow-publish-button": LocalJSX.ElsaWorkflowPublishButton & JSXBase.HTMLAttributes<HTMLElsaWorkflowPublishButtonElement>;
            "elsa-workflow-settings-modal": LocalJSX.ElsaWorkflowSettingsModal & JSXBase.HTMLAttributes<HTMLElsaWorkflowSettingsModalElement>;
        }
    }
}
