import Util from "./Util";
import VeggieClass from "./VeggieClass";

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
export default class SpawnController extends cc.Component {


    @property(cc.Prefab)
    veggiePrefab:cc.Prefab[] = [];

    @property(cc.Node)
    spawnPoint:cc.Node[] = [];

    emptySpot:cc.Node[] = [];
    
    veggieMap:Map<number,number> = new Map;

    // LIFE-CYCLE CALLBACKS:

    // onLoad () {}

    spawnFirst7Veg(){
        let vegs:cc.Node[] = [];

        vegs.push(cc.instantiate(this.veggiePrefab[0]));
        vegs.push(cc.instantiate(this.veggiePrefab[0]));
        vegs.push(cc.instantiate(this.veggiePrefab[1]));
        vegs.push(cc.instantiate(this.veggiePrefab[1]));
        vegs.push(cc.instantiate(this.veggiePrefab[2]));
        vegs.push(cc.instantiate(this.veggiePrefab[2]));
        vegs.push(cc.instantiate(this.veggiePrefab[Util.getRandom(3)]));

        Util.shuffleArray(this.spawnPoint);

        for(let i = 0; i<vegs.length; i++){
            this.spawnPoint[i].addChild(vegs[i]);
            this.addVeggieToMap(vegs[i]);
        }

        this.emptySpot.push(this.spawnPoint[7]);
    }

    spawnMore(){
        if(this.emptySpot.length > 1){

            do{
                let newVeg = cc.instantiate(this.veggiePrefab[Util.getRandom(3)]);
                this.addVeggieToMap(newVeg);
        
                Util.shuffleArray(this.emptySpot);
    
                let empty = this.emptySpot.pop();
                empty.addChild(newVeg);
                    
            }while(this.emptySpot.length > 1)
        }
    }

    removeVeggieFromMap(veggieType:number){
        if(this.veggieMap.has(veggieType)){
            let currentNum = this.veggieMap.get(veggieType);
            this.veggieMap.set(veggieType, currentNum-1);
        }
    }

    addVeggieToMap(veggie:cc.Node){
        if(this.veggieMap.has(veggie.getComponent(VeggieClass).vegType)){
            let currentNum = this.veggieMap.get(veggie.getComponent(VeggieClass).vegType);

            this.veggieMap.set(veggie.getComponent(VeggieClass).vegType, currentNum+1);
        }else{
            this.veggieMap.set(veggie.getComponent(VeggieClass).vegType, 1);
        }
    }

    reset(){
        this.emptySpot = [];
        this.veggieMap.clear();

        this.spawnPoint.forEach((child)=>{
            if(child.childrenCount > 0){
                child.destroyAllChildren();
            }
        });
    }

    // update (dt) {}
}
