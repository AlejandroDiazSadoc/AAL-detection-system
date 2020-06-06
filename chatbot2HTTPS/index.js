const express = require('express');
const app = express();
require('dotenv').config();
const apiai = require('apiai')(process.env.APIAI);
APIAI_SESSION_ID="myID"
const port = process.env.PORT || 5000;
const https = require("https"),
fs = require("fs");

const options = {
  key: fs.readFileSync("./privatekey.pem"),
  cert: fs.readFileSync("./certificate.pem")
};

//app.use(express.static(__dirname + '/views')); 
//app.use(express.static(__dirname + '/public')); 
app.use(express.static(__dirname));
//const server = app.listen(port);
const server = https.createServer(options, app).listen(port);
app.get('/', (req, res) => {
  res.sendFile('index.html');
});





const io = require('socket.io')(server);
io.on('connection', function(socket){
  console.log('usuario conectado');
});


io.on('connection', function(socket) {
    socket.on('mensaje', (text) => {
      let apiaiReq = apiai.textRequest(text, {
        sessionId: APIAI_SESSION_ID
      });
  
      apiaiReq.on('response', (response) => {
        let aiText = response.result.fulfillment.speech;
        socket.emit('bot respuesta', aiText);
      });
  
      apiaiReq.on('error', (error) => {
        console.log(error);
      });
  
      apiaiReq.end();
  
    });
  });
  