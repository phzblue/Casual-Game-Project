import GameController from "./GameController";
import BlockController from "./BlockController";

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
export default class MouseController extends cc.Component {

    static instance:MouseController = null;

    dragSpeed = 1;
    currentBlock:cc.Node = null;
    prevYPos = null;

    autoSpawn:boolean = true;
    hint3appear:Function = null;

    onLoad(){
        MouseController.instance = this;
    }

    start () {
        this.node.on(cc.Node.EventType.TOUCH_MOVE, (event)=>{
            if(!GameController.instance.isGameOver){
                if(this.currentBlock != null){

                    if(GameController.instance.hint1.activeInHierarchy){
                        GameController.instance.hint1.active = false;
                        GameController.instance.hint2.active = true;
    
                        this.hint3appear = ()=>{
                            GameController.instance.hint2.active = false;
                            GameController.instance.hint3.active = true;
                        };
    
                        this.scheduleOnce(this.hint3appear,3);
                    }
    
                    let touches = event.getTouches();
                    let touchLoc = touches[0].getLocation();
    
                    let localPos = this.node.parent.convertToNodeSpaceAR(touchLoc);
    
                    if(this.prevYPos != null){
                        let yDiff = localPos.y - this.prevYPos;
    
                        if(Math.floor(yDiff) > 0){
                            this.currentBlock.children[0].angle += Math.abs(yDiff);
                            //this.currentBlock.angle += Math.abs(yDiff);
    
                        }else if(Math.floor(yDiff) < 0){
                            this.currentBlock.children[0].angle -= Math.abs(yDiff);
    
                            //this.currentBlock.angle -= Math.abs(yDiff);
                        }
                    }
    
                    this.prevYPos = localPos.y;                
    
                    //this.currentBlock.getComponent(RotateObject).rotate = false;
    
                    if(Math.abs(localPos.x) < this.node.getContentSize().width/2 && Math.abs(localPos.x) > 0){
                        this.currentBlock.children[0].setPosition(localPos.x,this.currentBlock.children[0].position.y);
                    }
                }
            }
        },this.node);

        this.node.on(cc.Node.EventType.TOUCH_END, (event)=>{
            if(!GameController.instance.isGameOver){
                if(this.currentBlock != null){

                    if(GameController.instance.hint1.activeInHierarchy){
                        GameController.instance.hint1.active = false;
                    }
                    if(GameController.instance.hint2.activeInHierarchy){
                        GameController.instance.hint2.active = false;
                        this.unschedule(this.hint3appear);
                    }
    
                    if(GameController.instance.hint3.activeInHierarchy){
                        GameController.instance.hint3.active = false;
                    }
    
                    let prevBlock = this.currentBlock;
                    this.scheduleOnce(()=>{
                        //if(this.autoSpawn){
                            console.log("auto spawn");                        
                            // prevBlock.getComponentInChildren(BlockController).endObjectTurn();
                            GameController.instance.spawnBlock();
                        //}
                    },3);
    
                    //this.currentBlock.getComponent(RotateObject).rotate = false;
    
                    this.currentBlock.getComponentInChildren(cc.RigidBody).linearVelocity = new cc.Vec2(0,0);
                    this.currentBlock.getComponentInChildren(cc.RigidBody).gravityScale = 2;
                    this.currentBlock = null;
                }
            }
            
            
        }, this.node);
    }
}
