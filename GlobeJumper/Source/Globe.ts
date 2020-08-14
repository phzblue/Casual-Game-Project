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
export default class Globe extends cc.Component {

    @property(cc.Prefab)
    goldPrefab:cc.Prefab = null;

    radius:number = 295;
    rotationAngle:number = 3;

    onLoad(){
        // this.radius = this.getComponent(cc.PhysicsCircleCollider).radius;
    }

    start () {
        this.addGold();
        this.addGold();
    }

    addGold(){
        let angle = Util.getRandom(1200);
        let radian = cc.misc.degreesToRadians(angle);
        let x = this.radius * Math.sin(radian);
        let y = this.radius * Math.cos(radian);

        let newGold:cc.Node = cc.instantiate(this.goldPrefab);
        newGold.angle = -angle;
        newGold.getComponent(cc.BoxCollider).enabled = false;
        newGold.setParent(this.node);
        newGold.setPosition(x,y);

        cc.tween(newGold)
        .delay(.5)
        .then(cc.fadeIn(0.2))
        .call(()=>{
            newGold.getComponent(cc.BoxCollider).enabled = true;
        })
        .start();
    }

    moveGlobe(direction:number){
        if(!GameController.instance.isGameOver){
            this.node.angle += (this.rotationAngle * direction);
            if(direction != 0 && GameController.instance.jumper.scaleX != -direction){
                GameController.instance.jumper.scaleX = -direction;
            }
        }

        // this.node.children.forEach(element => {
        //     if(element.name.toLowerCase().includes("gold")){
        //         element.getComponent(cc.RigidBody).syncPosition(false);
        //     }
        // });
    }
}
