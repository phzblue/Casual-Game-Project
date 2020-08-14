import GameController from "./GameController";

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
    stick:cc.Node = null;

    lastPositionX:number = 0;
    lastPositionY:number = 0;
    speed:number = 1400;
    moveDirectionX:number = 0;
    moveDirectionY:number = 0;

    leftEdge:number;
    canMove:boolean = false;

    onLoad(){
        TouchController.instance = this;
    }

    start () {

        this.node.on(cc.Node.EventType.TOUCH_START, (touch, event)=>{
            this.lastPositionX = touch.getLocation().x;
            this.lastPositionY = touch.getLocation().y;
            this.canMove = true;

        },this.node)

        this.node.on(cc.Node.EventType.TOUCH_MOVE, (touch, event)=>{
            this.moveDirectionX = Math.sign(touch.getLocation().x - this.lastPositionX);
            this.moveDirectionY = Math.sign(touch.getLocation().y - this.lastPositionY);
            this.lastPositionX = touch.getLocation().x;
            this.lastPositionY = touch.getLocation().y;
        },this.node)

        this.node.on(cc.Node.EventType.TOUCH_END, (event)=>{
            this.moveDirectionX = 0;
            this.moveDirectionY = 0;
            this.canMove = false;
        }, this.node);

        this.node.on(cc.Node.EventType.TOUCH_CANCEL, (event)=>{
            this.moveDirectionX = 0;
            this.moveDirectionY = 0;
            this.canMove = false;
        }, this.node);
    }

    update(dt){
        if (this.canMove && !GameController.instance.isGameOver){ 
            let newPosX = this.stick.x + this.moveDirectionX * this.speed * dt;
            let newPosY = this.stick.y + this.moveDirectionY * this.speed * dt;

            //-800 and 600
            let exceedY = newPosY < -800 || newPosY > 600;
            if(exceedY){
                newPosY = this.stick.y;
            }

            //this.stick.setPosition(exceedX ? this.stick.x : newPosX, exceedY ? this.stick.y : newPosY);

            if(newPosX < (this.node.getContentSize().width / 2) && newPosX > -(this.node.getContentSize().width /2)){
                this.stick.setPosition(newPosX, newPosY);
                this.stick.getComponent(cc.RigidBody).angularVelocity += 5*this.moveDirectionX;                 
            }else{
                this.stick.setPosition(this.stick.x, newPosY);
            }

            this.moveDirectionX = 0;
            this.moveDirectionY = 0;
        }
    }
}
