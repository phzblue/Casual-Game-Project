import GameController from "./GameController";
import SpawnController from "./SpawnController";

// Learn TypeScript:
//  - [Chinese] https://docs.cocos.com/creator/manual/zh/scripting/typescript.html
//  - [English] http://www.cocos2d-x.org/docs/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - [Chinese] https://docs.cocos.com/creator/manual/zh/scripting/reference/attributes.html
//  - [English] http://www.cocos2d-x.org/docs/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - [Chinese] https://docs.cocos.com/creator/manual/zh/scripting/life-cycle-callbacks.html
//  - [English] http://www.cocos2d-x.org/docs/creator/manual/en/scripting/life-cycle-callbacks.html

const {ccclass, property} = cc._decorator;

@ccclass
export default class BunniController extends cc.Component {

    direction:number = 1;
    speed:number = 200;

    parentObj:cc.Node = null;

    jumped:boolean = false;
    canHarvest:boolean = false;

    @property(cc.SpriteFrame)
    idleSprite:cc.SpriteFrame = null;
    @property(cc.SpriteFrame)
    squeezeSprite:cc.SpriteFrame = null;

    onLoad(){
        this.parentObj = this.node.parent;
        //this.parentObj.setPosition(this.parentObj.position.x, cc.winSize.height * 30/100)
    }

    increaseSpeed(){
        this.speed += 25;
    }

    reset(){
        this.speed = 200;
        this.jumped = false;
        this.canHarvest = false;
    }

    onCollisionEnter(other:cc.Collider, self:cc.Collider){
        if(other.node.name.toLowerCase().includes("wall")){
            this.direction *= -1;
            this.parentObj.scaleX *= -1;

            GameController.instance.spawnController.getComponent(SpawnController).spawnMore();  
        }
    }

    update (dt) {
        if(!GameController.instance.isGameOver){
            this.parentObj.setPosition(this.parentObj.position.x + (this.speed*this.direction*dt),this.parentObj.position.y)
        }
    }
}
