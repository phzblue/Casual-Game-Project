import GameController from "./GameController";
import PlayerController from "./PlayerController";

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
export default class TouchController extends cc.Component {

    static instance:TouchController = null;

    @property(cc.Node)
    playerObj:cc.Node = null;

    lastPosition:number = 0;
    speed:number = 500;
    moveDirection:number = 0;

    leftEdge:number;
    canMove:boolean = true;

    onLoad(){
        TouchController.instance = this;
    }

    start () {

        this.node.on(cc.Node.EventType.TOUCH_START, (touch, event)=>{
            this.lastPosition = touch.getLocation().x;
        },this.node)

        this.node.on(cc.Node.EventType.TOUCH_MOVE, (touch, event)=>{
            this.moveDirection = Math.sign(touch.getLocation().x - this.lastPosition);
            this.lastPosition = touch.getLocation().x;
        },this.node)

        this.node.on(cc.Node.EventType.TOUCH_END, (event)=>{
            this.moveDirection = 0;
        }, this.node);

        this.node.on(cc.Node.EventType.TOUCH_CANCEL, (event)=>{
            this.moveDirection = 0;
        }, this.node);
    }

    update(dt){
        if (this.canMove && !GameController.instance.isGameOver && this.moveDirection != 0){
            PlayerController.instance.direction = this.moveDirection;
            let newPos = this.playerObj.x + this.moveDirection * this.speed * dt;

            if(newPos < (this.node.getContentSize().width / 2) && newPos > -(this.node.getContentSize().width /2)){
                this.playerObj.setPosition(newPos, this.playerObj.y);
            }else{
                this.playerObj.setPosition(this.playerObj.x, this.playerObj.y);
            }

            this.moveDirection = 0;
        }
    }
}
