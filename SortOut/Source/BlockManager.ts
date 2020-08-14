import Util from "./Util";
import BlockBase from "./BlockBase";
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

@ccclass
export default class BlockManager extends cc.Component {
    blockPrefab: cc.Prefab = null;

    colorBlockContainer:Map<string,cc.Node> = new Map();
    colorBlockCoordinates:string[] = [];

    exitBlockContainer:Map<string,cc.Node> = new Map();
    exitBlockCoordinates:string[] = [];

    exitCode:number[] = [0,1,2,3,4];

    obstacleThreshold:number = 0;
    currentColorAmount:number = 4;
    addLastColor:boolean = false;

    lightSFX:cc.AudioClip = null;
    
    setAudio(audio){
        this.lightSFX = audio;
    }

    resetGame(){
        this.exitCode = [0,1,2,3,4];
        this.addLastColor = false;
        this.obstacleThreshold = 0;

        this.exitBlockCoordinates = [];
        this.colorBlockCoordinates = [];

        this.exitCode = [0,1,2,3,4];
        this.currentColorAmount = 4;

        this.addLastColor = false;

        this.colorBlockContainer.clear();
        this.exitBlockContainer.clear();
    }

    generatePuzzleBlock(blockContainer:cc.Node, cellBlock:number){
        blockContainer.width = 131 * cellBlock + (4 * cellBlock) + 15;
        blockContainer.height = 135 * cellBlock + 15;

        for(let i = 0; i<cellBlock; i++ ){
            for(let j = 0; j<cellBlock; j++){
                let child = cc.instantiate(this.blockPrefab);
                blockContainer.addChild(child);
                if(i == 0 || j == cellBlock-1 || j == 0 || i == cellBlock-1){
                    //this is exit

                    if(!((i+","+j).match("0,0") ||
                    (i+","+j).match("0,"+(cellBlock-1)) ||
                    (i+","+j).match((cellBlock-1)+","+(cellBlock-1)) ||
                    (i+","+j).match((cellBlock-1)+",0"))){
                        this.exitBlockContainer.set(i+","+j,child);
                        this.exitBlockCoordinates.push(i+","+j);
                    }

                    child.getComponent(cc.Sprite).enabled = false;
                    child.getComponent(BlockBase).isExit = true;
                }else{
                    child.getComponent(BlockBase).setBlockRandomColor(this.currentColorAmount);

                    this.colorBlockContainer.set(i+","+j,child);
                    this.colorBlockCoordinates.push(i+","+j);
                }
            }
        }

    }

    generateExit(cellNum:number = 6, exitColor: number = -1){
        let r = Util.getRandom(this.exitBlockCoordinates.length);
        this.exitBlockContainer.get(this.exitBlockCoordinates[r]).getComponent(BlockBase).isActive = true;

        if(exitColor != -1){
            this.exitBlockContainer.get(this.exitBlockCoordinates[r]).getComponent(BlockBase).setExitColor(exitColor);
        }else{
            this.exitBlockContainer.get(this.exitBlockCoordinates[r]).getComponent(BlockBase).setExitColor(this.exitCode.shift());
        }

        if(this.exitBlockCoordinates[r].endsWith("0")){
            this.exitBlockContainer.get(this.exitBlockCoordinates[r]).getComponent(BlockBase).setExitBlockRotation(90);
        }else if(this.exitBlockCoordinates[r].endsWith((cellNum-1).toString())){
            this.exitBlockContainer.get(this.exitBlockCoordinates[r]).getComponent(BlockBase).setExitBlockRotation(-90);
        }else if(this.exitBlockCoordinates[r].startsWith((cellNum-1).toString())){
            this.exitBlockContainer.get(this.exitBlockCoordinates[r]).getComponent(BlockBase).setExitBlockRotation(-180);
        }

        this.exitBlockContainer.get(this.exitBlockCoordinates[r]).getComponent(cc.Sprite).enabled = true;
    }

    generateFreeBlock(){
        let r:string = Util.getRandomArrayObject(this.colorBlockCoordinates);

        let vec2Coor = Util.stringToVector(r);
        this.colorBlockContainer.get(r).getComponent(BlockBase).resetBlock();

        return vec2Coor;
    }

    addNewExit(cellNum:number){
        if(!this.addLastColor){
            this.currentColorAmount++;
            this.addLastColor = true;
            this.generateExit(cellNum, -1);
        }
    }

    generateNewColorBlock(currFreeSpot:cc.Vec2, newFreeSpot:string, updateFreePos:Function){

        let container = [newFreeSpot, (currFreeSpot.x+","+currFreeSpot.y)];
        Util.shuffleArray(container);

        let chosenSpot = container.pop();

        this.colorBlockContainer.get(chosenSpot).getComponent(BlockBase).playShinyAnim(true);
        this.colorBlockContainer.get(chosenSpot).getComponent(BlockBase).playSpawnAnimation(this.currentColorAmount, this.blockPrefab, (color)=>{           
            let freeCoor = container.pop();
            this.checkNearExit(chosenSpot,color,(a)=>{
                if(a){
                    cc.audioEngine.playEffect(this.lightSFX,false);
                    this.colorBlockContainer.get(chosenSpot).getComponent(BlockBase).playShinyAnim(false);
                }
                
                updateFreePos(Util.stringToVector(freeCoor));              
            });
        });
        
    }

    respawnExitBlock(exitBlock:BlockBase, exitIndex:number, colorCode:number){
        exitBlock.removeExit(exitIndex);
        this.generateExit(6,colorCode); 
    }

    generateHiddenBlock(freeCoor:string){

        let coor = null;
        do{
            coor = Util.getRandomArrayObject(this.colorBlockCoordinates);
            if(coor == freeCoor || this.colorBlockContainer.get(coor).getComponent(BlockBase).isObstacleBlock){
                coor = null;
            }else{
                this.colorBlockContainer.get(coor).getComponent(BlockBase).setHiddenAnimation();
                break;
            }

        }while(coor == null)
    }

    convertColorBlockToObstacle(score:number, currFreeCoor:string, startCountdown:Function){
        if(Math.floor(score/30) > this.obstacleThreshold){
            this.obstacleThreshold++;

            let coor = null;
            do{
                coor = Util.getRandomArrayObject(this.colorBlockCoordinates);
                if(coor != currFreeCoor){
                    this.colorBlockContainer.get(Util.getRandomCollectionObject(
                        this.colorBlockContainer)).getComponent(BlockBase).setBlockToObstacle();
                        startCountdown();
                        break;
                }
            }while(true)
            
        }
    }

    checkAllExit(){
        for(let coor of this.colorBlockCoordinates){
            let block:BlockBase = this.colorBlockContainer.get(coor).getComponent(BlockBase);

            this.checkNearExit(coor,block.color,(a)=>{
                if(a){
                    cc.audioEngine.playEffect(this.lightSFX,false);
                    block.playShinyAnim(false);
                }
                              
            });
        }
    }

    checkNearExit(currPos:string, currColor:number, special:Function){

        let test:boolean = true;

        for(let coor of this.exitBlockCoordinates){

            if(this.exitBlockContainer.get(coor).getComponent(BlockBase).isActive){

                let result = Util.stringToVector(coor).sub(Util.stringToVector(currPos));
                if(Util.vectorToString(result).indexOf("1") !== -1 && Util.vectorToString(result).indexOf("0") !== -1 &&
                this.exitBlockContainer.get(coor).getComponent(BlockBase).exitColor.indexOf(currColor) !== -1){
                    special(true);
                    return;
                }
            }
        }
        special(false);
    }

    findObstacleBlock(){
        for(let block of this.colorBlockContainer.values()){
            if(block.getComponent(BlockBase).isObstacleBlock){
                block.getComponent(BlockBase).isObstacleBlock = false;
                block.getComponent(BlockBase).setBlockRandomColor(this.currentColorAmount);
            }
        }
    }
}
