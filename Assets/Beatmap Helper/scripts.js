var beatTimestamps = new Set();
var parameterList = [];
var instructionList = [];
var audioPlayer = null;
var timestampList = null;
var timestampCache = null;
var beatDisplayToggle = null;
var currentTimestampIndex = 0;
var prevTime = Number.POSITIVE_INFINITY;
var timestampBuffer = [];
var bufferSize = 0;
var curAudioFile = null;

//  =============================== How To Add A New Instruction ===============================
//
//  1. Add a new instruction in index.html <table> element in <div id='instructionguide'>
//  2. Add an option in scripts.js 
//     in the following 2 functions : updateList(), importList(instructionL,parameterL) 
//     -> just add a new option for <select> in variable instructionSelector 
//  3. Add a new case in Assets/Scripts/SongManager.cs function LoadBeatmap(string beatmapName)
//
//                                                                              by Weng,Yu-Hsin
//  ============================================================================================

function downloadString(filename, text) {
    var element = document.createElement('a');
    element.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(text));
    element.setAttribute('download', filename);

    element.style.display = 'none';
    document.body.appendChild(element);

    element.click();

    document.body.removeChild(element);
}

function init(){
    audioPlayer = document.getElementById("audio");
    timestampList = document.getElementById("timestampList");
    document.getElementById("upload").addEventListener("change", handleFiles, false);
    document.getElementsByTagName("body")[0].addEventListener("keydown", addTimestamp);
    document.getElementById("showtable").addEventListener("change",function(){
        console.log("checkbox change");
        if(this.checked == true){
            document.getElementById("instructionguide").style.display = "none";
        }else{
            document.getElementById("instructionguide").style.display = "block";
        }
    });
    audioPlayer.ontimeupdate = function() { 
        if (beatDisplayToggle.checked) highlightCurrentTimestamp(); 
        prevTime = audioPlayer.currentTime; 
    };
    beatDisplayToggle = document.getElementById("beatDisplayToggle");
    beatDisplayToggle.onchange = function() { 
        if(beatDisplayToggle.checked) {
            cacheTimestamps();
            highlightCurrentTimestamp();
        }
        else{
            updateList();
        }
    };

    document.getElementById("bpm").onchange = function() {
        if(document.getElementById("BPMdisplayToggle").checked){
            updateList();

        }
    };


    document.getElementById("timestampBufferSize").addEventListener("change", () => {
        let newBufferSize = document.getElementById("timestampBufferSize").value;
        if(newBufferSize === "" || newBufferSize < 1) document.getElementById("timestampBufferSize").value = 1;
        while(bufferSize > newBufferSize) {
            timestampBuffer.shift();
            bufferSize--;
        }
        updateList();
    });

    document.getElementById("export").addEventListener("click", () => {
        exportTimestamps();
    });
}

function handleFiles(event) {
    var files = event.target.files;
    curAudioFile = files[0];
    $("#src").attr("src", URL.createObjectURL(files[0]));
    audioPlayer.load();
    beatDisplayToggle.checked = false;
    clearAllTimestamps();
}

function addTimestamp(e){
    if((e.code !== "Space" && e.code !=="KeyZ") || beatDisplayToggle.checked) return;
    if(audioPlayer.readyState >= 2){
        beatTimestamps.add(audioPlayer.currentTime);
        instructionList.push('ROTATE_FORWARD_TO');
        parameterList.push('');
        while(document.getElementById("timestampBufferSize").value <= bufferSize){
            timestampBuffer.shift();
            bufferSize--;
        }
        timestampBuffer.push(audioPlayer.currentTime);
        bufferSize++;
        if(beatTimestamps.size === 1) currentTimestamp = audioPlayer.currentTime;
        
        updateList(2);
    }
}


function renewInstructionParameterList(){
        
        instructionList = [];
        parameterList = [];
        let index = 0;
        for(let timestamp of beatTimestamps){

            let instr = document.getElementById('instruction'+index.toString()).value;
            let param = document.getElementById('parameter'+index.toString()).value;
            
            instructionList.push(instr);
            parameterList.push(param);

            index ++;
        }   

}

function removeTimestamp(timestamp){
    if(beatDisplayToggle.checked) return;
    beatTimestamps.delete(timestamp);
    timestampBuffer = timestampBuffer.filter(t => t !== timestamp);
    bufferSize = timestampBuffer.length;
    updateList();
}


function updateList(flag = 0){
    
    
    let listdata = "Timestamps:";
    if(document.getElementById("enableTimestampToggle").checked){
        if(!flag){
            renewInstructionParameterList();
        }
        if(document.getElementById("BPMdisplayToggle").checked){
            let BPM = document.getElementById("bpm").value;
            if(BPM === "") {
                alert("BPM unspecified. Using default BPM = 120");
                BPM = 120;
            }
            let secPerBeat = 1 / (BPM / 60);
            let index = 0;
            for(let timestamp of beatTimestamps){
                let instructionSelector = '<label for="instruction">Instruction:</label>\n<select name="instruction" id="instruction' + index.toString() + '"' + '><option value="ROTATE_FORWARD_TO">ROTATE_FORWARD_TO</option>\n<option value="ROTATE_FORWARD_BY">ROTATE_FORWARD_BY</option>\n<option value="ROTATE_UP_TO">ROTATE_UP_TO</option>\n<option value="ROTATE_UP_BY">ROTATE_UP_BY</option>\n';
                                
                instructionSelector += '<option value="ORBITNOTE_FIXED_RED">ORBITNOTE_FIXED_RED</option>\n<option value="ORBITNOTE_FIXED">ORBITNOTE_FIXED</option>\n';
                instructionSelector += '<option value="ORBITNOTE_MOVING_RED">ORBITNOTE_MOVING_RED</option>\n<option value="ORBITNOTE_MOVING">ORBITNOTE_MOVING</option>\n';
                instructionSelector += '<option value="TAPNOTE">TAPNOTE</option>\n';

                instructionSelector += '</select>';
                let parameter = '<label for="parameter"style="margin-left:3px;margin-right:3px;">Parameters:</label><input type="text" id="parameter' + index.toString() + '" name="parameter"  cols="3" style="resize:none;overflow-y:hidden;">';
                
                listdata += `<li id=${timestamp} onclick="removeTimestamp(${timestamp});">${(Math.floor(timestamp / secPerBeat*4)/4).toFixed(2)}</li>`;
                listdata += instructionSelector;
                listdata += parameter;

                index ++;
            }

        }
        else{
            let index = 0;
            for(let timestamp of beatTimestamps){
                let instructionSelector = '<label for="instruction">Instruction:</label>\n<select name="instruction" id="instruction' + index.toString() + '"' + '><option value="ROTATE_FORWARD_TO">ROTATE_FORWARD_TO</option>\n<option value="ROTATE_FORWARD_BY">ROTATE_FORWARD_BY</option>\n<option value="ROTATE_UP_TO">ROTATE_UP_TO</option>\n<option value="ROTATE_UP_BY">ROTATE_UP_BY</option>\n';
                                
                instructionSelector += '<option value="ORBITNOTE_FIXED_RED">ORBITNOTE_FIXED_RED</option>\n<option value="ORBITNOTE_FIXED">ORBITNOTE_FIXED</option>\n';
                instructionSelector += '<option value="ORBITNOTE_MOVING_RED">ORBITNOTE_MOVING_RED</option>\n<option value="ORBITNOTE_MOVING">ORBITNOTE_MOVING</option>\n';
                instructionSelector += '<option value="TAPNOTE">TAPNOTE</option>\n';

                instructionSelector += '</select>';
                
                let parameter = '<label for="parameter"style="margin-left:3px;margin-right:3px;">Parameters:</label><input type="text" id="parameter' + index.toString() + '" name="parameter"  cols="3" style="resize:none;overflow-y:hidden;">';
                
                listdata += `<li id=${timestamp} onclick="removeTimestamp(${timestamp});">${timestamp}</li>`;
                listdata += instructionSelector;
                listdata += parameter;
                index ++;
            }
        }
        timestampList.innerHTML = listdata;
        
            let index = 0;
            for(let timestamp of beatTimestamps){
                document.getElementById('instruction' + index.toString()).value =  instructionList[index];
                document.getElementById('parameter' + index.toString()).value = parameterList[index]; 
                document.getElementById('instruction' + index.toString()).addEventListener('change',function(){
                    
                    let id = this.id;
                    id = id.split('n');
                    id = parseInt(id[2]);
                    console.log('instruction'+id+'change');
                    instructionList[id] = this.value;
                });
                document.getElementById('parameter' + index.toString()).addEventListener('change',function(){
                    let id = this.id;
                    id = id.split('r');
                    id = parseInt(id[2]);
                    console.log('parameter'+id+'change');
                    parameterList[id] = this.value;
                });    
                index ++;     
            }
        
    }
    else{

        if(flag){
            if(flag != 2){
                renewInstructionParameterList();
            }
        }
        listdata += " (Hidden)";
        timestampList.innerHTML = listdata;
    }

    

    // ! Spaghetti Code !
    if(document.getElementById("BPMdisplayToggle").checked){
        let BPM = document.getElementById("bpm").value;
        if(BPM === "") {
            alert("BPM unspecified. Using default BPM = 120");
            BPM = 120;
        }
        let secPerBeat = 1 / (BPM / 60);
            
        let listdata2 = "";
        listdata2 = "Timestamp Buffer:";
        for(let timestamp of timestampBuffer){
            listdata2 += `<li onclick="removeTimestamp(${timestamp});">${(Math.floor(timestamp / secPerBeat*4)/4).toFixed(2)}</li>`;
        }
        document.getElementById("timestampBufferList").innerHTML = listdata2;
    }
    else{
        
        let listdata2 = "";
        listdata2 = "Timestamp Buffer:";
        for(let timestamp of timestampBuffer){
            listdata2 += `<li onclick="removeTimestamp(${timestamp});">${timestamp}</li>`;
        }
        document.getElementById("timestampBufferList").innerHTML = listdata2;
    }

}

function importList(instructionL,parameterL){
    let listdata = "Timestamps:";
    if(document.getElementById("enableTimestampToggle").checked){

        if(document.getElementById("BPMdisplayToggle").checked){
            let BPM = document.getElementById("bpm").value;
            if(BPM === "") {
                alert("BPM unspecified. Using default BPM = 120");
                BPM = 120;
            }
            let secPerBeat = 1 / (BPM / 60);
            let index = 0;
            for(let timestamp of beatTimestamps){

                let instructionSelector = '<label for="instruction">Instruction:</label>\n<select name="instruction" id="instruction' + index.toString() + '"' + '><option value="ROTATE_FORWARD_TO">ROTATE_FORWARD_TO</option>\n<option value="ROTATE_FORWARD_BY">ROTATE_FORWARD_BY</option>\n<option value="ROTATE_UP_TO">ROTATE_UP_TO</option>\n<option value="ROTATE_UP_BY">ROTATE_UP_BY</option>\n';
                                
                instructionSelector += '<option value="ORBITNOTE_FIXED_RED">ORBITNOTE_FIXED_RED</option>\n<option value="ORBITNOTE_FIXED">ORBITNOTE_FIXED</option>\n';
                instructionSelector += '<option value="ORBITNOTE_MOVING_RED">ORBITNOTE_MOVING_RED</option>\n<option value="ORBITNOTE_MOVING">ORBITNOTE_MOVING</option>\n';
                instructionSelector += '<option value="TAPNOTE">TAPNOTE</option>\n';

                instructionSelector += '</select>';
                let parameter = '<label for="parameter"style="margin-left:3px;margin-right:3px;">Parameters:</label><input type="text" id="parameter' + index.toString() + '" name="parameter"  cols="3" style="resize:none;overflow-y:hidden;">';
                
                listdata += `<li id=${timestamp} onclick="removeTimestamp(${timestamp});">${(Math.floor(timestamp / secPerBeat*4)/4).toFixed(2)}</li>`;
                listdata += instructionSelector;
                listdata += parameter;
                index ++;
            }

        }
        else{
            let index = 0;
            for(let timestamp of beatTimestamps){
                let instructionSelector = '<label for="instruction">Instruction:</label>\n<select name="instruction" id="instruction' + index.toString() + '"' + '><option value="ROTATE_FORWARD_TO">ROTATE_FORWARD_TO</option>\n<option value="ROTATE_FORWARD_BY">ROTATE_FORWARD_BY</option>\n<option value="ROTATE_UP_TO">ROTATE_UP_TO</option>\n<option value="ROTATE_UP_BY">ROTATE_UP_BY</option>\n';
                                
                instructionSelector += '<option value="ORBITNOTE_FIXED_RED">ORBITNOTE_FIXED_RED</option>\n<option value="ORBITNOTE_FIXED">ORBITNOTE_FIXED</option>\n';
                instructionSelector += '<option value="ORBITNOTE_MOVING_RED">ORBITNOTE_MOVING_RED</option>\n<option value="ORBITNOTE_MOVING">ORBITNOTE_MOVING</option>\n';
                instructionSelector += '<option value="TAPNOTE">TAPNOTE</option>\n';

                instructionSelector += '</select>';
                let parameter = '<label for="parameter"style="margin-left:3px;margin-right:3px;">Parameters:</label><input type="text" id="parameter' + index.toString() + '" name="parameter"  cols="3" style="resize:none;overflow-y:hidden;">';
                
                listdata += `<li id=${timestamp} onclick="removeTimestamp(${timestamp});">${timestamp}</li>`;
                listdata += instructionSelector;
                listdata += parameter;
                index ++;
            }
        }
        
    }
    else{
        listdata += " (Hidden)";
    }

    timestampList.innerHTML = listdata;

    let index = 0;
    for(let timestamp of beatTimestamps){
        
        try{
            console.log("instruction");
            document.getElementById("instruction" + index.toString()).value =  instructionL[index];
            console.log("parameter");
            document.getElementById("parameter" + index.toString()).value = parameterL[index++]; 
        }catch(e){
            console.log(index);
            console.log(e);
        }      

    }

    // ! Spaghetti Code !
    if(document.getElementById("BPMdisplayToggle").checked){
        let BPM = document.getElementById("bpm").value;
        if(BPM === "") {
            alert("BPM unspecified. Using default BPM = 120");
            BPM = 120;
        }
        let secPerBeat = 1 / (BPM / 60);
            
        let listdata2 = "";
        listdata2 = "Timestamp Buffer:";
        for(let timestamp of timestampBuffer){
            listdata2 += `<li onclick="removeTimestamp(${timestamp});">${(Math.floor(timestamp / secPerBeat*4)/4).toFixed(2)}</li>`;
        }
        document.getElementById("timestampBufferList").innerHTML = listdata2;
    }
    else{
        
        let listdata2 = "";
        listdata2 = "Timestamp Buffer:";
        for(let timestamp of timestampBuffer){
            listdata2 += `<li onclick="removeTimestamp(${timestamp});">${timestamp}</li>`;
        }
        document.getElementById("timestampBufferList").innerHTML = listdata2;
    }

}

function cacheTimestamps(){
    timestampCache = Array.from(beatTimestamps);
    currentTimestampIndex = 0;
    prevTime = Number.POSITIVE_INFINITY;
}

function highlightCurrentTimestamp(){
    let prevTimestampIndex = currentTimestampIndex;
    if(audioPlayer.currentTime < prevTime){ // Restart search from beginning.
        for(currentTimestampIndex = 0; currentTimestampIndex < timestampCache.length; currentTimestampIndex++){
            if(timestampCache[currentTimestampIndex] > audioPlayer.currentTime){
                console.log("new timestamp index " + currentTimestampIndex);
                if(prevTimestampIndex < timestampCache.length) {
                    document.getElementById(timestampCache[prevTimestampIndex].toString()).removeAttribute("style");
                    console.log("removed highlight from " + timestampCache[prevTimestampIndex]);
                }
                document.getElementById(timestampCache[currentTimestampIndex].toString()).setAttribute("style", "background-color:white;color:black");
                
                break;
            }
        }
    }
    else if(currentTimestampIndex < timestampCache.length && timestampCache[currentTimestampIndex] <= audioPlayer.currentTime){
        if(prevTimestampIndex < timestampCache.length) {
            document.getElementById(timestampCache[prevTimestampIndex].toString()).removeAttribute("style");
            console.log("removed highlight from " + timestampCache[prevTimestampIndex]);
        }
        while(currentTimestampIndex < timestampCache.length){
            if(timestampCache[currentTimestampIndex] > audioPlayer.currentTime){
                console.log("new timestamp index " + currentTimestampIndex);
                
                document.getElementById(timestampCache[currentTimestampIndex].toString()).setAttribute("style", "background-color:white;color:black");
                
                break;
            }
            currentTimestampIndex++;
        }
    }

}

function clearAllTimestamps(){
    if(beatDisplayToggle.checked) return;
    beatTimestamps = new Set();
    timestampBuffer = [];
    instructionList = [];
    parameterList = [];
    updateList();
    currentTimestampIndex = 0;
    bufferSize = 0;

}

/** Export the sequence of timestamps into .txt.
 * 
*/
function exportTimestamps(){
    if(audioPlayer.readyState > 0){
        let BPM = document.getElementById("bpm").value;
        if(BPM === "") {
            alert("BPM unspecified. Using default BPM = 120");
            BPM = 120;
        }
        let secPerBeat = 1 / (BPM / 60);
        let filename = curAudioFile.name.substr(0, curAudioFile.name.lastIndexOf('.'));
        let raw = "BPM "+`${BPM}\n`;
        let index = 0;
        for(let timestamp of beatTimestamps){
            let instruction = document.getElementById("instruction"+index.toString()).value;
            let parameters = document.getElementById("parameter"+index.toString()).value;
            parameters = parameters.split(',').join(' ');
            let newline = instruction + ' ' + `${(Math.floor(timestamp / secPerBeat * 4) / 4).toFixed(2)}` + ' ' + parameters + '\n';
            raw += newline;
            index ++;
        }

        downloadString(filename, raw);
    }
    else alert("No audio file loaded. Download aborted.");
}

function importTimestamps(){
    clearAllTimestamps();
    let handle = new FileReader();
    handle.onload = () => {
        let tokens = handle.result.split('\n');
        let BPM = parseInt(tokens[0].split(' ')[1]);
        document.getElementById("bpm").value = BPM;
        tokens.shift();
        let secPerBeat = 1 / (BPM / 60);
        

        for(let line of tokens){
            if(line == ""){
                console.log('empty line');
            }else if(line[0]=='/' && line[1]=='/'){
                console.log(line);
            }else if(line[0]=='#'){
                console.log(line);
            }else{
                console.log(line);
                line = line.split(' ');
                let instr = line[0];
                line.shift();
                let time = line[0];
                line.shift();
                let param = line.join(',');
                
                if(!isNaN(parseFloat(time))){
                    
                    beatTimestamps.add(parseFloat(time) * secPerBeat);
                    instructionList.push(instr);
                    parameterList.push(param);
                } 
            }
            
        }
        console.log(beatTimestamps);
        console.log(instructionList);
        console.log(parameterList);
        importList(instructionList,parameterList);
    };
    handle.readAsText(document.getElementById("import").files[0]);
}