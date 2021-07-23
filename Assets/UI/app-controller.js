var airConsole;
var INPUT_LEFT = 276;
var INPUT_RIGHT = 275;
var INPUT_UP = 273;
var INPUT_DOWN = 274;
var INPUT_ROTATE = 114; // R
var INPUT_INSERT= 105; // I
var INPUT_START = 115; // S
var INPUT_SELECT = 32; // space

const player = {
    color: "#bf4b73",
    active: false
}
function App() {
    me = this;
    me.airConsole = new AirConsole({orientation: AirConsole.ORIENTATION_LANDSCAPE});
    
    me.airConsole.onReady = function(code) {
        console.log("onReady", code);
    };
    
    me.airConsole.onConnect = function(deviceId) {
        console.log(me.airConsole.getProfilePicture(deviceId));
        setTargetImage(me.airConsole.getProfilePicture(deviceId));
        me.airConsole.message(AirConsole.SCREEN, {
            device_id: deviceId,
            user_name: me.airConsole.getNickname(deviceId)
        });
    };

    me.airConsole.onDeviceStateChange = function(device_id, device_data) {
        console.log("onDeviceStateChange", device_id, device_data);
    };

    if (!("ontouchstart" in document.createElement("div"))) {
        var elements = document.getElementsByTagName("*");
        for (var i = 0; i < elements.length; ++i) {
            var element = elements[i];
            var ontouchstart = element.getAttribute("ontouchstart");
            if (ontouchstart) {
                element.setAttribute("onmousedown", ontouchstart);
            }
            var ontouchend = element.getAttribute("ontouchend");
            if (ontouchend) {
                element.setAttribute("onmouseup", ontouchend);
            }
        }
    }

    me.airConsole.onMessage = function(from, data) {
        if (data["action"] === "UPDATE_STATE") {
            setActive(data["active"])
            if (data["active"]) {
                me.airConsole.vibrate(100);
            }
            if (data["color"]) setBackground(data["color"])
        }
        if (data["action"] === "UPDATE_STATE_CARD") {
            console.log("onMessage", from, data);
            setTargetImageBase64(data["card_url"])
            setSnippet(data["card_snippet"])
        }
        if (data["action"] === "SET_MASTER_PLAYER") {
            console.log("is_master:" +data["is_master"])
            setMaster(data["is_master"]);
        }
    };
    
    updateController(player);
}

App.prototype.sendEvent = function(event) {
    console.log(event);
    this.airConsole.message(AirConsole.SCREEN, {"action": event});
};

App.prototype.startgame = function() {
    console.log(event);
    this.airConsole.message(AirConsole.SCREEN, {"action": INPUT_START});
    document.getElementById("btn-play").style.display = "none";
};

const setBackground = (color) => {
    const root = document.documentElement;
    root.style.setProperty('--user-color', color);
}

const setTargetImage = (url) => {
    if( document.getElementById("target")) {
        document.getElementById("target").disabled = false
    }
    document.getElementById("target").src = url;
}

const setTargetImageBase64 = (base64Img) => {
    document.getElementById('target')
        .setAttribute(
            'src',
            'data:image/png;base64,' + base64Img
        );
}

const setSnippet = (text) => {
    if (document.getElementById("snippet").disabled) {
        document.getElementById("snippet").disabled = false;
    }
    
    document.getElementById("snippet").innerHTML = text;
}

const setActive = (isActive) => {
    console.log("setting element axctive = " + isActive)
    console.log(isActive)
    if(isActive){
        document.getElementById("wrapper").classList.remove('inactive');
        document.getElementById("btn-play").style.display = "none";
    } else {
        document.getElementById("wrapper").classList.add('inactive');
    }
    console.log(document.getElementById("wrapper").classList);
}

const setMaster = (isMaster) => {
    console.log("setting element master = " + isMaster)
    console.log(isMaster)
    if(isMaster){
        console.log("found master!")
        document.getElementById("btn-play").style.transform = "scale(100%)";
    } else {
        document.getElementById("btn-play").style.transform = "scale(0)";
    }
}

const toogleButton = (id) =>  {
    console.log("toggling id:" + id)
    var x = document.getElementById(id);
    if (x.style.display === "none") {
        x.style.display = "block";
    } else {
        x.style.display = "none";
    }
}

const disableById = (id) => {
    document.getElementById(id).classList.add('inactive');
}

const enableById = (id) => {
    document.getElementById(id).classList.remove('inactive');
}

const updateController = (player) => {
    setBackground(player.color);
    if (player.target) {
        setTargetImage(player.target);
    }
    setActive(player.active);
}