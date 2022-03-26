// Never console.log objects directly, only values.
// Always display a lot of helpers to understand the state
// and the space
//import { Network } from "./sync.js";
import { Network } from "https://metahack.glitch.me/sync.js";

let sync;
initNetwork();

function initNetwork() {
  console.log('init network...');

  const adapter = Network["webrtc"];
  adapter.init("phone-tracker.glitch.me"); // TODO room
  // another idea is to use QRCode to communicate between phones
  // with each front camera pointing at the other screen
  // so there is no need for a signalling server
  // there is also bluetooth, or NFC (https://developer.mozilla.org/en-US/docs/Web/API/Web_NFC_API)

  //adapter.send(JSON.stringify({message:"hello"}));
  //adapter.sendTo(JSON.stringify({message:"hello"}), userID)

  const listeners = {};
  adapter.onmessage = (event) => {
    const userID = event.userID;
    const data = JSON.parse(event.data);
    if ('evt' in data && data.evt in listeners) {
      for (let i=0; i<listeners[data.evt].length; i++) {
        listeners[data.evt][i](data, userID);
      }
    } else {
      console.log(event);
    }
  };
  adapter.onconnectionstatechange = (state, userID) => {
    console.log("state changed "+ state + " " + userID);
    switch(state) {
      case "connected": // The connection has become fully connected
        //adapter.sendTo(JSON.stringify({message:"hello"}), userID);
        break;
      case "disconnected":
      case "failed": // One or more transports has terminated unexpectedly or in an error
      case 'close': // The connection has been closed
        document.body.style.background = "#ffffff"
        if (window.metahack.onquit) window.metahack.onquit(userID);
        break;
    }
  };
  
  
  adapter.onready = (userID) => {
    if (window.metahack.onready) window.metahack.onready(userID);
    console.log("onready "+userID);
    adapter.send(JSON.stringify({"evt":"test"}), userID);
  };

  // close network when the page is closed
  window.addEventListener("beforeunload", (event) => {
    adapter.close();
  });

  window.metahack = {
    send: (msg, toid) => adapter.send(JSON.stringify(msg), toid),
    on: (evt, callback) => {
      if (evt in listeners) {
        listeners[evt].push(callback);
      } else {
        listeners[evt] = [callback];
      }
    }
  };

  window.metahack.on('test', (data, userID) => {
    document.body.style.background = "#777777"
    //console.log(data.str+' from '+userID);
    window.metahack.send({"evt":"hello"}, userID);
  });
}