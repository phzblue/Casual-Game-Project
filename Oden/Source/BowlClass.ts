// Learn TypeScript:
//  - https://docs.cocos.com/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - https://docs.cocos.com/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - https://docs.cocos.com/creator/manual/en/scripting/life-cycle-callbacks.html

import GameController from "./GameController";
import Util from "./Util";

const {ccclass, property} = cc._decorator;

@ccclass
export default class BowlClass extends cc.Component {

    @property(cc.SpriteFrame)
    odenSprites:cc.SpriteFrame[] = [];

    @property(cc.Node)
    row1Container:cc.Node = null;
    @property(cc.Node)
    row2Container:cc.Node = null;

    @property({
        type:cc.AudioClip
    })
    bowlSfx:cc.AudioClip = null;

    onCollisionEnter(other:cc.Collider, self:cc.Collider){
        if(other.node.name.toLowerCase() == "stick" && GameController.instance.stick.checkDangos()){
            GameController.instance.scoreController.comboScore();
            GameController.instance.stick.clearStick();
            this.moveBowlUp(true);
        }
    }

    addOdenToSoup(odenType:number){
        let newOden = new cc.Node();
        newOden.addComponent(cc.Sprite).spriteFrame = this.odenSprites[odenType]

        if(this.row1Container.childrenCount<1){
            this.row1Container.addChild(newOden);
        }else{
            this.row2Container.addChild(newOden);
        }
    }

    reset(){
        this.node.setPosition(0,-420);
        this.row1Container.destroyAllChildren();
        this.row2Container.destroyAllChildren();
    }

    moveBowlUp(isReset:boolean){
        cc.tween(this.node).stop();
        if(!isReset){
            let randX = Util.getRandom(this.node.parent.getContentSize().width/2 - (this.node.getContentSize().width),
            -(this.node.parent.getContentSize().width/2)) + (this.node.getContentSize().width);

            cc.tween(this.node)
            .call(
                ()=>{
                    this.node.getComponent(cc.BoxCollider).enabled = false;
                    this.node.setPosition(randX, this.node.y);
                }
            )
            .to(0.5, { position: cc.v2(randX,-60)})
            .call(
                ()=>{
                    this.node.getComponent(cc.BoxCollider).enabled = true;
                }
            )
            .start()
        }else{
            cc.audioEngine.playEffect(this.bowlSfx,false);
            this.node.getComponent(cc.BoxCollider).enabled = false;

            cc.tween(this.node)
            .delay(1)
            .to(0.1, {
                position: cc.v2(this.node.x,-420)
            })
            .call(()=>{
                this.row1Container.destroyAllChildren();
                this.row2Container.destroyAllChildren();
                this.node.getComponent(cc.BoxCollider).enabled = true;

            })
            .start()
        }
    }
}
