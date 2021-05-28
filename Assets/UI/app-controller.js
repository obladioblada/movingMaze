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
    target: 'https://picsum.photos/150/150.jpg',
    active: false
}
function App() {
    me = this;
    me.airConsole = new AirConsole({orientation: AirConsole.ORIENTATION_LANDSCAPE});
    
    me.airConsole.onReady = function(code) {
        console.log("onReady", code);
    };
    
    me.airConsole.onConnect = function(deviceId) {
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
        console.log("onMessage", from, data);
        if (data["action"] === "UPDATE_STATE") {
            console.log("updating state", from, data);
            if (data["state"] === "move") {
                toogleButton("rotate");
            } else if (data["state"] === "shift") {
                toogleButton("rotate");
            } else {
                if(data["color"])  setBackground(data["color"])
                if(data["active"]) setActive(data["active"])
            }
            
        }
    };
    
    updateController(player);
}

App.prototype.sendEvent = function(event) {
    console.log(event);
    this.airConsole.message(AirConsole.SCREEN, {"action": event});
};

const setBackground = (color) => {
    const root = document.documentElement;
    root.style.setProperty('--user-color', color);
}

const setTargetImage = (url) => {
    document.getElementById("target").src = url;
}

const setActive = (isActive) => {
    console.log(isActive)
    console.log(isActive == true)
    if(isActive){
        document.getElementById("wrapper").classList.remove('inactive');
    } else {
        document.getElementById("wrapper").classList.add('inactive');
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
    setTargetImage(player.target);
    setActive(player.active);
}