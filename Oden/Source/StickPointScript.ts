// Learn TypeScript:
//  - https://docs.cocos.com/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - https://docs.cocos.com/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - https://docs.cocos.com/creator/manual/en/scripting/life-cycle-callbacks.html

import GameController from "./GameController";
import StickClass from "./StickClass";
import DangoClass from "./DangoClass";

const {ccclass, property} = cc._decorator;

@ccclass
export default class StickPointScript extends cc.Component {

    @property({
        type:cc.AudioClip
    })
    pokeSfx:cc.AudioClip = null;

    dangoStick:StickClass = null;

    onLoad(){
        this.dangoStick = GameController.instance.stick;
    }

    onCollisionEnter(other:cc.Collider, self:cc.Collider){
        if(other.node.name.toLowerCase().includes("oden") && this.dangoStick.dangoContainer.childrenCount<3){
            
            cc.audioEngine.playEffect(this.pokeSfx,false);

            GameController.instance.scoreController.normalScore(other.node);            
            
            other.node.removeComponent(cc.RigidBody);
            other.node.removeComponent(cc.BoxCollider);
            other.node.getComponent(cc.Sprite).enabled = false; 
            other.node.getComponentInChildren(cc.Animation).play();           
            
            this.dangoStick.addDango(other.node.getComponent(DangoClass).type);

            if(this.dangoStick.checkDangos()){
                GameController.instance.bowlObj.moveBowlUp(false);                
            }

            let odenNode = other.node;
            cc.tween(this.node).delay(0.5).call(()=>{odenNode.destroy();}).start();
        }
    }
}
