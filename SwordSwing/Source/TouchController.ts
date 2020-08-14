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

    static instance:TouchController = null;

    @property(cc.Node)
    slayer:cc.Node = null;

    lastPositionX:number = 0;
    speed:number = 1400;
    moveDirectionX:number = 0;

    leftEdge:number;
    canMove:boolean = false;
    currentDirection:number = 1;

    onLoad(){
        TouchController.instance = this;
    }

    start () {
        this.node.on(cc.Node.EventType.TOUCH_START, (touch, event)=>{
            this.lastPositionX = touch.getLocation().x;
            // this.lastPositionY = touch.getLocation().y;
            this.canMove = true;

        },this.node)

        this.node.on(cc.Node.EventType.TOUCH_MOVE, (touch, event)=>{
            this.moveDirectionX = Math.sign(touch.getLocation().x - this.lastPositionX);
            // this.moveDirectionY = Math.sign(touch.getLocation().y - this.lastPositionY);
            this.lastPositionX = touch.getLocation().x;
            // this.lastPositionY = touch.getLocation().y;
        },this.node)

        this.node.on(cc.Node.EventType.TOUCH_END, (event)=>{
            this.moveDirectionX = 0;
            // this.moveDirectionY = 0;
            this.canMove = false;
        }, this.node);

        this.node.on(cc.Node.EventType.TOUCH_CANCEL, (event)=>{
            this.moveDirectionX = 0;
            // this.moveDirectionY = 0;
            this.canMove = false;
        }, this.node);
    }

    update(dt){
        if (this.canMove && !GameController.instance.isGameOver){ 
            let newPosX = this.slayer.x + this.moveDirectionX * this.speed * dt;

            //this.stick.setPosition(exceedX ? this.stick.x : newPosX, exceedY ? this.stick.y : newPosY);

            if(newPosX < (this.node.getContentSize().width / 2) && newPosX > -(this.node.getContentSize().width /2)){
                this.slayer.setPosition(newPosX, this.slayer.y);
                if(this.moveDirectionX != 0 && this.currentDirection != this.moveDirectionX){
                    this.currentDirection = this.moveDirectionX;
                    this.slayer.getChildByName("sword").getComponent(cc.RigidBody).angularVelocity = 0;      
                    this.slayer.getChildByName("sword").getComponent(cc.RigidBody).angularVelocity += 200*this.moveDirectionX;                 
                }else{
                    this.slayer.getChildByName("sword").getComponent(cc.RigidBody).angularVelocity += 30*this.moveDirectionX;                 
                }

                this.slayer.children.forEach(element => {
                    if(element.getComponent(cc.RigidBody) != null){
                        element.getComponent(cc.RigidBody).syncPosition(false);                        
                    }
                });

            }else{
                this.slayer.setPosition(this.slayer.x, this.slayer.y);
            }

            this.moveDirectionX = 0;
        }
    }
}
