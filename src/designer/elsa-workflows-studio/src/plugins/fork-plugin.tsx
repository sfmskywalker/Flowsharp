﻿import {ElsaPlugin} from "../services/elsa-plugin";
import {eventBus} from '../services/event-bus';
import {ActivityDescriptorDisplayContext, ActivityDesignDisplayContext, EventTypes} from "../models";
import {h} from "@stencil/core";
import {parseJson} from "../utils/utils";
import {ForkIcon} from "../components/icons/fork-icon";

export class ForkPlugin implements ElsaPlugin {
  constructor() {
    eventBus.on(EventTypes.ActivityDescriptorDisplaying, this.onActivityDescriptorDisplaying);
    eventBus.on(EventTypes.ActivityDesignDisplaying, this.onActivityDesignDisplaying);
  }

  onActivityDescriptorDisplaying(context: ActivityDescriptorDisplayContext) {
    const descriptor = context.activityDescriptor;

    if (descriptor.type !== 'Fork')
      return;

    context.activityIcon = <ForkIcon/>;
  }

  onActivityDesignDisplaying(context: ActivityDesignDisplayContext) {
    const activityModel = context.activityModel;

    if (activityModel.type !== 'Fork')
      return;

    const props = activityModel.properties || [];
    const branches = props.find(x => x.name == 'Branches') || { expression: '' };
    const expression = branches.expression;
    const branchList = parseJson(expression) || [];

    context.activityIcon = <ForkIcon/>;
    context.outcomes = branchList;
  }
}