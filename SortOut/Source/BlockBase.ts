import Util from "./Util";
import AnimationMethod from "./AnimationMethod";
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

export enum BlockColor{
    GREEN,
    PURPLE,
    YELLOW,
    ORANGE,
    MINT,
    FREE,
    BLOCKED
}

@ccclass
export default class BlockBase extends cc.Component {

    public isExit: boolean = false;
    public isActive: boolean = false;
    public isHiddenBlock: boolean = false;
    public isObstacleBlock: boolean = false;

    @property(cc.Prefab)
    prefab: cc.Prefab = null;

    @property(cc.SpriteFrame)
    shinySpriteFrame: cc.SpriteFrame[] = [];
    @property(cc.SpriteFrame)
    colorSpriteFrame: cc.SpriteFrame[] = [];
    @property(cc.SpriteFrame)
    hiddenSpriteFrame: cc.SpriteFrame = null;
    @property(cc.SpriteFrame)
    obstacleSpriteFrame: cc.SpriteFrame = null;

    @property(cc.SpriteFrame)
    exitSpriteFrame: cc.SpriteFrame[] = [];
    @property(cc.Node)
    exitSpriteContainer:cc.Node = null;

    exitColor:number[] = [];
    color:number = 6;

    colorPicker:string[] = ["#2c5456","#bb90ab","#dfca3a","#e37036","#83ac85","#dee8e8"]


    setBlockToObstacle(){
        this.isObstacleBlock = true;
        this.color = 6;
        this.node.getComponent(cc.Sprite).spriteFrame = this.obstacleSpriteFrame;
    }

    setBlockToHidden(){
        this.isHiddenBlock = true;
        this.node.getComponent(cc.Sprite).spriteFrame = this.hiddenSpriteFrame;
        if(this.node.children[1] != null){
            this.node.children[1].getComponent(cc.Sprite).spriteFrame = this.shinySpriteFrame[this.color];
        }
    }

    removeExit(index:number){
        this.exitSpriteContainer.removeAllChildren();
        this.exitColor.splice(index,1);
        //todo add back color exit to exit container
        for(let x of this.exitColor){
            this.setExitColor(x,false);
        }

        if(this.exitColor.length == 0){
            this.isActive = false;
        }
    }

    setBlockRandomColor(num:number){
        let rand:number = Util.getRandom(num);
        this.setColor(rand);
    }

    setExitBlockRotation(num:number){
        this.node.angle = (num);
    }

    setExitColor(color:number, push:boolean = true){
        if(push)
            this.exitColor.push(color);

        let newExit = new cc.Node();
        newExit.addComponent(cc.Sprite).spriteFrame = this.exitSpriteFrame[color];
        this.exitSpriteContainer.addChild(newExit);
    }

    resetBlock(){
        this.isHiddenBlock = false;
        this.color = BlockColor.FREE;
        this.node.getComponent(cc.Sprite).spriteFrame = null;
    }

    setColor(num:number, isHidden:boolean = false){
        this.color = num;
        this.isHiddenBlock = isHidden;

        if(this.node.children[1] != null){
            this.node.children[1].getComponent(cc.Sprite).spriteFrame = this.shinySpriteFrame[num];
        }

        if(!isHidden){
            this.node.getComponent(cc.Sprite).spriteFrame = this.colorSpriteFrame[num];
            
        }else{
            this.node.getComponent(cc.Sprite).spriteFrame = this.hiddenSpriteFrame;
        }
    }

    setColorFromHidden(){
        this.isHiddenBlock = false;
        this.node.getComponent(cc.Sprite).spriteFrame = this.colorSpriteFrame[this.color];
    }

    setHiddenAnimation(){
        let fadeIn = cc.fadeIn(0.2);
        let fadeOut = cc.fadeOut(0.2);
        let changeSprite = cc.callFunc(this.setBlockToHidden,this);
        let seq = cc.sequence(cc.repeat(cc.sequence(fadeOut,fadeIn),2),changeSprite,fadeIn);

        this.node.runAction(seq);
    }

    setUnhiddenAnimation(f:Function){
        this.isHiddenBlock = false;
        let fadeIn = cc.fadeIn(.2);
        let fadeOut = cc.fadeOut(.2);
        let changeSprite = cc.callFunc(this.setColorFromHidden,this);
        let seq = cc.sequence(fadeOut,fadeIn,changeSprite,fadeIn, cc.callFunc(f));

        this.node.runAction(seq);
    }

    playMoveAnimation(isExit:boolean, direction, changeSprite:Function){
        this.node.getComponent(cc.Sprite).spriteFrame = null;

        //UP,DOWN,LEFT,RIGHT
        let loc = null;
        let size:number = isExit ? 135*10 : 135;
        switch(direction){
            case 0:
                loc = new cc.Vec2(0,size);
                break;
            case 1:
                loc = new cc.Vec2(0,-size);
                break;
            case 2:
                loc = new cc.Vec2(-size,0);
                break;
            case 3:
                loc = new cc.Vec2(size,0);
                break;
        }
        let newSprite = cc.instantiate(this.prefab); 
        newSprite.getComponentInChildren(BlockBase).setColor(this.color, this.isHiddenBlock);
        if(this.isHiddenBlock){
            newSprite.getComponent(cc.MotionStreak).color = new cc.Color().fromHEX(this.colorPicker[5]);
        }else{
            newSprite.getComponent(cc.MotionStreak).color = new cc.Color().fromHEX(this.colorPicker[this.color]);
        }
        this.node.addChild(newSprite);

        
        newSprite.runAction(cc.sequence(cc.moveBy(isExit?.3:.05,newSprite.getPosition().add(loc)),
        cc.callFunc(changeSprite,this),  cc.callFunc(()=>{
            newSprite.removeFromParent(false);
        },this)));
    }

    scaleself(scaleTo:number, duration){
        return cc.scaleTo(duration,scaleTo);
    }

    playSpawnAnimation(color:number, prefab:cc.Prefab, finishAnim:Function){

        if(this.node.children[1] != null){
            this.node.children[1].active = false;
            this.node.children[1].getComponent(AnimationMethod).toggleFadeAnimation(true);
        }

        this.node.getComponent(cc.Sprite).spriteFrame = null;
        let rand:number = Util.getRandom(color);

        let newSprite = cc.instantiate(prefab);
        newSprite.getComponent(BlockBase).setColor(rand,false);
        newSprite.scale = 0.5;
        this.node.addChild(newSprite);

        let removeSelf = cc.callFunc(()=>{
            finishAnim(rand);
            newSprite.removeFromParent(false);
        });

        let activeSprite = cc.callFunc(()=>{
            this.setColor(rand);
        });

        newSprite.runAction(cc.sequence(newSprite.getComponent(BlockBase).scaleself(1,.15),activeSprite,removeSelf));
    }

    playShinyAnim(stopAnim:boolean){
        if(this.node.children[1] != null){
            this.node.children[1].active = !stopAnim;
            this.node.getComponentInChildren(AnimationMethod).toggleFadeAnimation(stopAnim);
        }
    }
}
