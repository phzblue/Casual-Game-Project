// Learn TypeScript:
//  - https://docs.cocos.com/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - https://docs.cocos.com/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - https://docs.cocos.com/creator/manual/en/scripting/life-cycle-callbacks.html

import DangoClass from "./DangoClass";
import Util from "./Util";
import GameController from "./GameController";

const {ccclass, property} = cc._decorator;

@ccclass
export default class SpawnController extends cc.Component {

    @property(cc.Prefab)
    dango:cc.Prefab[] = [];

    spawnContainer:cc.Node[] = [];
    canSpawn:boolean = false;
    timer:number = 0;

    onLoad(){
        this.spawnContainer = this.node.children;
    }

    startSpawn () {
        this.canSpawn = true;
        this.generateDango();
    }

    generateDango(){
        let positions:any[] = this.spawnContainer.slice(0,this.spawnContainer.length);

        for(let i = 0; i < positions.length-5; i++){
            let newDango = cc.instantiate(this.dango[Util.getRandom(3)]);
            let posObj:cc.Node = this.pickPosition(positions);
            posObj.addChild(newDango);
            newDango.getComponent(DangoClass).addVelocity();
        }
    }

    pickPosition(array:any[]){
        Util.shuffleArray(array)
        return array.shift();
    }

    reset(){
        this.canSpawn = false;
        this.spawnContainer.forEach(element => {
            element.destroyAllChildren();
        });
    }

    update (dt) {
        if(this.canSpawn && !GameController.instance.isGameOver){
            if(this.timer>=GameController.spawnInterval){
                this.generateDango();
                this.timer = 0;
            }
            
            this.timer+=dt;
        }
    }
}
