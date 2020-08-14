// Learn TypeScript:
//  - https://docs.cocos.com/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - https://docs.cocos.com/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - https://docs.cocos.com/creator/manual/en/scripting/life-cycle-callbacks.html

import GameController from "./GameController";
import DangoClass, { DangoType } from "./DangoClass";

const {ccclass, property} = cc._decorator;

@ccclass
export default class StickClass extends cc.Component {

    @property(cc.Node)
    dangoContainer:cc.Node = null;

    @property(cc.SpriteFrame)
    odenSprites:cc.SpriteFrame[] = [];

    odenTypeInStick:number[] = [];

    addDango(odenType:number){
        let newOden = new cc.Node();
        newOden.addComponent(cc.Sprite).spriteFrame = this.odenSprites[odenType]
        this.dangoContainer.addChild(newOden);
        this.odenTypeInStick.push(odenType);
    }

    checkDangos(){
        return this.dangoContainer.childrenCount == 3;
    }

    countCombo(){
        let countMap:Map<number, number> = new Map();

        this.odenTypeInStick.forEach(element => {
            if(countMap.has(element)){
                let curr = countMap.get(element) + 1;
                countMap.delete(element);
                countMap.set(element, curr++);
            }else{
                countMap.set(element, 1);
            }
            
            this.addOdenToBowl(element);
        });
        
        let combo = 0;

        countMap.forEach((value, key)=>{
            if(value > combo){
                combo = value;
            }
        });

        return combo;
    }

    addOdenToBowl(odenType:number){
        GameController.instance.bowlObj.addOdenToSoup(odenType);
    }

    reset(){
        this.clearStick();
        this.node.angle = 0;
        this.node.setPosition(0,this.node.y)
        this.node.getComponent(cc.RigidBody).angularVelocity = 0;
    }
    
    clearStick(){
        this.dangoContainer.destroyAllChildren();
        this.odenTypeInStick = [];
        
    }
}
