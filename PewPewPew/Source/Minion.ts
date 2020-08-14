// Learn TypeScript:
//  - https://docs.cocos.com/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - https://docs.cocos.com/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - https://docs.cocos.com/creator/manual/en/scripting/life-cycle-callbacks.html

import Util from "./Util";
import GameController from "./GameController";

const {ccclass, property} = cc._decorator;

@ccclass
export default class Minion extends cc.Component {

    @property()
    speedMultiplier:number = 1;
    canMove:boolean = false;

    changeDirection(){
        this.node.scaleX *= -1;
    }

    update (dt) {
        if(!GameController.instance.isGameOver && this.canMove){
            this.node.angle += (this.speedMultiplier * GameController.instance.globeController.currentGlobeDirection);
            this.node.setPosition(
                Util.calculatePosFromAngleRadius(GameController.instance.globeController.globeRadius,-this.node.angle)
            );
        }

        if(this.node.scaleX != GameController.instance.globeController.currentGlobeDirection){
            this.node.scaleX = GameController.instance.globeController.currentGlobeDirection;
        }
        
    }
}
