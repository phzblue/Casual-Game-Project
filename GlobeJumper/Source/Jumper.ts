// Learn TypeScript:
//  - https://docs.cocos.com/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - https://docs.cocos.com/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - https://docs.cocos.com/creator/manual/en/scripting/life-cycle-callbacks.html

import GameController from "./GameController";

const {ccclass, property} = cc._decorator;

@ccclass
export default class Jumper extends cc.Component {

    @property({
        type: cc.AudioClip
    })
    jumpSFX:cc.AudioClip = null;
    @property({
        type: cc.AudioClip
    })
    ghostSFX:cc.AudioClip = null;
    @property({
        type: cc.AudioClip
    })
    coinSFX:cc.AudioClip = null;

    @property(cc.ParticleSystem)
    particleEffects:cc.ParticleSystem = null;
    @property(cc.ParticleSystem)
    sparkEffects:cc.ParticleSystem = null;

    tweenObj:cc.Tween = null;
    
    start () {
        this.tweenObj = cc.tween(this.node)
        // .delay(.5)
        // .to(0.3, {position : cc.v2(0,385)})
        .repeatForever(
            //cc.jumpBy(1,cc.v2(0,0),200,1)
            cc.sequence(
                cc.spawn(
                    cc.moveBy(.5,0,270),
                    cc.callFunc(()=>{
                        cc.audioEngine.play(this.jumpSFX,false,1);
                        this.particleEffects.resetSystem();
                        this.node.getComponent(cc.Animation).play();
                    })
                ),
                cc.moveBy(.5,0,-270),
            )
        ).start();
    }

    stopTween(){
        this.tweenObj.stop();
    }

    onCollisionEnter(other:cc.Collider, self:cc.Collider){
        if(other.node.name.toLowerCase().includes("gold")){
            cc.audioEngine.play(this.coinSFX,false,1);

            this.sparkEffects.resetSystem();
            other.node.getComponent(cc.BoxCollider).enabled = false;
            other.node.destroy();
            GameController.instance.scoreController.addScore();
            GameController.instance.globeController.addGold();
        }

        if(other.node.name.toLowerCase().includes("obstacle")){
            cc.audioEngine.play(this.ghostSFX,false,1);

            GameController.instance.scoreController.deductScore();
            other.node.getComponent(cc.Animation).play();
        }
    }

    // update (dt) {}
}
