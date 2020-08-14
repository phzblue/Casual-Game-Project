// Learn TypeScript:
//  - https://docs.cocos.com/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - https://docs.cocos.com/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - https://docs.cocos.com/creator/manual/en/scripting/life-cycle-callbacks.html

import Util from "./Util";

const {ccclass, property} = cc._decorator;

export enum DangoType{
    ONE = 0,
    TWO = 1,
    THREE = 2
}

@ccclass
export default class DangoClass extends cc.Component {

    @property(cc.Enum)
    type:DangoType = 0;

    addVelocity(){
        this.node.getComponent(cc.RigidBody).linearVelocity = cc.v2(0, Util.getRandom(-150,-30));
    }

    stopAnim(){
        this.node.stopAllActions();
        cc.tween(this.node).stop();
    }
}
