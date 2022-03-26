export const Network = {
	websocket: {
	  init: function(host) {
		const self = this;
		self.sock = new WebSocket("wss://"+host+"/ws");
		if (!self.onmessage) self.onmessage = () => {};
		self.sock.onmessage = (message) => { self.onmessage(message); }; // TODO JSON.parse message
		self.sock.onopen = function(e) { console.log('Connected !'); }
		self.sock.onclose = function(e) {
		  console.log('Socket is closed. Reconnect will be attempted in 3 second.', e.reason);
		  delete self.sock;
		  self.sock = null;
		  setTimeout(function() {
			self.init(host);
		  }, 3000);
		};
	  },
	  //receive: function(callback) { }, 
	  send: function(message) { if (this.sock && this.sock.readyState === WebSocket.OPEN) this.sock.send(message); },
	  close: function() { if (this.sock) this.sock.close(); }
	},
	webrtc: {
	  /*
		Star configuration *-*, but there is only one connection between each peers because its bidirectionnal
		For each new peers that connect to the network, all existing peers connect to it
		This code works with only one only one data channel per peers
	  */
	  init: function(host) {
		const self = this;
		
		self.isready = {};
		self.onmessage = () => {}; // default empty callback
		self.onconnectionstatechange = (state, userID) => {}; // default empty callback
		self._onconnectionstatechange = (state, userID) => {
		  if (state === 'connected') {
			const f = () => {
			  if (self.isready[userID]) return;
			  self.send('ready?', userID);
			  setTimeout(f, 100);
			};
			f();
		  }
		  self.onconnectionstatechange(state, userID);
		};
		self.onready = (userId) => {};
		
		self.receiveCallback = (event, userID) => {
		  if (event.data === 'ready?') {
			self.send('ready!', userID);
			return;
		  }
		  if (event.data === 'ready!') {
			self.isready[userID] = true;
			if (self.onready) self.onready(userID);
			return;
		  }
		  
		  event.userID = userID;
		  if (self.onmessage) self.onmessage(event);
		};
		self.connections = {}; // list of all RTCPeerConnection
		self.channels = {}; // list of data channel for a RTCPeerConnection
		
		// if youd need ws for development: https://stackoverflow.com/questions/11768221/firefox-websocket-security-issue
		// firefox about:config enable network.websocket.allowInsecureFromHTTPS during insecure devs
		self.sock = new WebSocket("wss://"+host+"/wrtc");
		self.sock.onmessage = event => { self.signalReceive(JSON.parse(event.data)); };
		self.sock.onopen = console.log;
		self.sock.onclose = console.log;
	  },
	  initConnection: function(userID, hosting) {
		const self = this;
		
		const co = new RTCPeerConnection({
			'iceServers': [
			  {"urls":"stun:stun.l.google.com:19302"},
			  {"urls":"stun:stun1.l.google.com:19302"},
			  {"urls":"stun:stun2.l.google.com:19302"},
			  {"urls":"stun:stun3.l.google.com:19302"},
			  // {
			  //   "urls":"turn:[ADDRESS]:[PORT][?transport=udp]",
			  //   "username":"[USERNAME]",
			  //   "credential":"[CREDENTIAL]"
			  // }
			]
		  });
		self.connections[userID] = co;
		
		// https://developer.mozilla.org/en-US/docs/Web/API/RTCPeerConnection/connectionstatechange_event
		// not supported in firefox
		co.addEventListener('connectionstatechange', function(event) {
		  console.log("onconnectionstatechange");
		  self._onconnectionstatechange(co.connectionState, userID);
		  switch(co.connectionState) {
			case "connected": // The connection has become fully connected
			  console.log('connected to peer '+userID);
			  break;
			case "disconnected":
			case "failed": // One or more transports has terminated unexpectedly or in an error
			  console.log('disconnected of failed to contact a peer '+userID);
			  break;
			case "closed": // The connection has been closed
			  console.log('connection closed '+userID);
			  break;
		  }
		});
  
		co.oniceconnectionstatechange += console.log;
		co.onnegotiationneeded += event => {
		  console.log('negotiationneeded');
		  console.log(event);
		};
		
		co.onicecandidate = event => {
		  if (event.candidate) {
			self.signalSend({ to:userID, candidate:event.candidate }); // Send the candidate to the remote peer
		  } else {
			console.log('All ICE candidates have been sent');
		  }
		}
		
		if (hosting) { // Create the data channel and establish its event listeners
		  const channel = co.createDataChannel("sync");
		  self.channels[userID] = channel;
		  channel.onopen = () => self._onconnectionstatechange('connected', userID);
		  channel.onmessage = (msg) => { self.receiveCallback(msg, userID); };
		  channel.onclose = () => self._onconnectionstatechange('close', userID);
  
		  co.createOffer()
			.then(offer => co.setLocalDescription(offer))
			.then(() => self.signalSend({ to:userID, hostDescription:co.localDescription }));
		} else {
		  co.ondatachannel = event => {
			const channel = event.channel
			self.channels[userID] = channel;
			channel.onopen = () => self._onconnectionstatechange('connected', userID);
			channel.onmessage = (msg) => { self.receiveCallback(msg, userID); };
			channel.onclose = () => self._onconnectionstatechange('close', userID);
		  }
		}
	  },
	  signalReceive: function(event) { // receive event from signalling server
		//console.log(event)
		const self = this;
		
		// when a new user is connected to the signalling server
		if (event.newConnection) { // event.newConnection == userID
		  self.initConnection(event.newConnection, true);
		  return;
		}
		
		// as a remote, there is a need to create the connection before receiving messages
		if (!self.connections[event.from]) {
		  self.initConnection(event.from, false);
		}
		
		if (event.candidate) { // candidate exchange
		  self.connections[event.from].addIceCandidate(event.candidate).catch(console.log);
		} else if (event.remoteDescription) { // host get the remote description
		  //console.log('set remoteDescription')
		  self.connections[event.from].setRemoteDescription(event.remoteDescription).catch(console.log);
		} else if (event.hostDescription) { // remote get the host description
		  const co = self.connections[event.from];
		  co.setRemoteDescription(event.hostDescription)
			.then(() => co.createAnswer())
			.then(answer => co.setLocalDescription(answer))
			.then(() => self.signalSend({ to:event.from, remoteDescription:co.localDescription }))
			.catch(console.log);
		}
	  },
	  signalSend: function(message) {
		this.sock.send(JSON.stringify(message));
	  },
	  send: function(message, toid) {
		if (toid) {
		  const channel = this.channels[toid];
		  if (channel.readyState === "open") channel.send(message);
		  return;
		}
		for (const userID in this.channels) {
		  const channel = this.channels[userID];
		  if (channel.readyState === "open") channel.send(message);
		}
	  },
	  close: function() {
		for (const userID in this.channels) { this.channels[userID].close(); } // close channels
		for (const userID in this.connections) { this.connections[userID].close(); } // close connections
		this.sock.close();
	  }
	}
  };