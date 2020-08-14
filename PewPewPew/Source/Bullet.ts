// Learn TypeScript:
//  - https://docs.cocos.com/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - https://docs.cocos.com/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - https://docs.cocos.com/creator/manual/en/scripting/life-cycle-callbacks.html

import GameController from "./GameController";

const {ccclass, property} = cc._decorator;

@ccclass
export default class Bullet extends cc.Component {

    @property({
        type:cc.AudioClip
    })
    bulletSFX:cc.AudioClip = null;
    @property({
        type:cc.AudioClip
    })
    miniontSFX:cc.AudioClip = null;
    @property({
        type:cc.AudioClip
    })
    globeSFX:cc.AudioClip = null;

    hasHit:boolean = false;
    
    start(){
        cc.audioEngine.play(this.bulletSFX,false,1);
    }
    
    onCollisionEnter(other:cc.Collider, self:cc.Collider){
        if(!GameController.instance.isGameOver && !this.hasHit){
            this.hasHit = true;

            if(other.name.toLowerCase().includes("minion")){
                cc.audioEngine.play(this.miniontSFX,false,1);

                GameController.instance.scoreController.addScore();
                GameController.instance.globeController.addMinion();
                
                other.node.getComponent(cc.CircleCollider).enabled = false;
                other.node.getComponent(cc.Sprite).enabled = false;
                other.node.getComponentInChildren(cc.ParticleSystem).resetSystem();
    
                cc.tween(other.node)
                .delay(3)
                .call(()=>{
                    other.node.destroy();
                })
                .start();
            }else if(other.name.toLowerCase().includes("globe")){
                cc.audioEngine.play(this.globeSFX,false,1);

                GameController.instance.scoreController.deductScore();
            }

            this.node.destroy();
        }
    }

    // update (dt) {}
}
