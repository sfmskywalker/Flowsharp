# elsa-activity-picker-modal



<!-- Auto Generated Below -->


## Properties

| Property             | Attribute    | Description | Type                 | Default     |
| -------------------- | ------------ | ----------- | -------------------- | ----------- |
| `publishing`         | `publishing` |             | `boolean`            | `undefined` |
| `workflowDefinition` | --           |             | `WorkflowDefinition` | `undefined` |


## Events

| Event              | Description | Type               |
| ------------------ | ----------- | ------------------ |
| `publishClicked`   |             | `CustomEvent<any>` |
| `unPublishClicked` |             | `CustomEvent<any>` |


## Dependencies

### Used by

 - [elsa-workflow-editor](../elsa-workflow-editor)

### Graph
```mermaid
graph TD;
  elsa-workflow-editor --> elsa-workflow-publish-button
  style elsa-workflow-publish-button fill:#f9f,stroke:#333,stroke-width:4px
```

----------------------------------------------

*Built with [StencilJS](https://stenciljs.com/)*