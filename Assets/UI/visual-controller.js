let visualController
let airconsole

window.onload = function() {
    visualController = new VisualController(new AirConsole({orientation: AirConsole.ORIENTATION_LANDSCAPE}))
    console.log(visualController);
}

function VisualController(airConsole) {
    this.init_(airConsole);
    updateController(player);
}

VisualController.prototype.init_ = function (airConsole){
    console.log("init Visual Controller")
    console.log(airConsole)
    airconsole = airConsole

    airConsole.onReady = function(code) {
        console.log("onReady", code);
    };
    
    airConsole.onConnect = function(deviceId) {
        airconsole.message(AirConsole.SCREEN, {
            device_id: deviceId,
            user_name: airConsole.getNickname(deviceId)
        });
    };

    airConsole.onDeviceStateChange = function(device_id, device_data) {
        console.log("onDeviceStateChange", device_id, device_data);
    };

    airConsole.onMessage = function(from, data) {
        console.log("onMessage", from, data);
        if (data["action"] === "UPDATE_STATE") {
            console.log("updating state", from, data);
            setBackground(data["color"])
            setActive(data["active"])
        }
    };

    VisualController.prototype.sendMessageToScreen = function(msg) {
        airConsole.message(AirConsole.SCREEN, msg);
    };

}

const rotate = () => {airconsole.message(AirConsole.SCREEN, {action: "rotate"})}

const setBackground = (color) => {
    const root = document.documentElement;
    root.style.setProperty('--user-color', color);
};

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

const updateController = (player) => {
    setBackground(player.color);
    setTargetImage(player.target);
    setActive(player.active);
}

/* 
  Implementation:
  update the player object and call updateController
*/
const player = {
    color: "#bf4b73",
    target: 'https://picsum.photos/150/150.jpg',
    active: false
}