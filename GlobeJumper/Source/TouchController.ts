// Learn TypeScript:
//  - https://docs.cocos.com/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - https://docs.cocos.com/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - https://docs.cocos.com/creator/manual/en/scripting/life-cycle-callbacks.html

import GameController from "./GameController";

const {ccclass, property} = cc._decorator;

@ccclass
export default class TouchController extends cc.Component {

    lastPositionX:number = 0;

    start () {
        this.node.on(cc.Node.EventType.TOUCH_START, (touch, event)=>{
            this.lastPositionX = touch.getLocation().x;
        },this.node)

        this.node.on(cc.Node.EventType.TOUCH_MOVE, (touch, event)=>{
            GameController.instance.globeController.moveGlobe(Math.sign(touch.getLocation().x - this.lastPositionX));
            this.lastPositionX = touch.getLocation().x;
        },this.node)
    }

    // update (dt) {}
}
