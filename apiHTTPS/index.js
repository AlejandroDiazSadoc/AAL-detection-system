
const express = require('express');
const bodyParser = require('body-parser');
const cors = require('cors');
const helmet = require('helmet');
const morgan = require('morgan');
require('dotenv').config();
const jwt = require('express-jwt');
const jwksRsa = require('jwks-rsa');
const https = require("https"),
fs = require("fs");

const options = {
  key: fs.readFileSync("./privatekey.pem"),
  cert: fs.readFileSync("./certificate.pem")
};
var admin = require("firebase-admin");

var serviceAccount = require(process.env.FILE);

admin.initializeApp({
  credential: admin.credential.cert(serviceAccount),
  databaseURL: process.env.URL
});

const app = express();


app.use(helmet());


app.use(bodyParser.json());


app.use(cors());

app.use(morgan('combined'));


var jwtCheck = jwt({
      secret: jwksRsa.expressJwtSecret({
          cache: true,
          rateLimit: true,
          jwksRequestsPerMinute: 5,
          jwksUri: 'https://dev-ceqiayp2.eu.auth0.com/.well-known/jwks.json'
    }),
    audience: process.env.AUDIENCE,
    issuer: process.env.ISSUER,
    algorithms: ['RS256']
});

app.use(jwtCheck);

app.get('/PersonaCaida', function (req, res) {
var topic="miCasa"
console.log(topic);
  var payload = {
  notification: {
    title: "¡¡Se ha detectado una caida!!",
    body: "La persona es probable que se haya caído.",
  }
};
admin.messaging().sendToTopic(topic, payload)
  .then(function(response) {
    console.log("Successfully sent message:", response);
    res.send('Mensaje enviado correctamente!!');
  })
  .catch(function(error) {
    console.log("Error enviando el mensaje:", error);
    res.send('Error');
  });
});

app.get('/PersonaNoCome', function (req, res) {
var topic="miCasa"
console.log(topic);
  var payload = {
  notification: {
    title: "¡¡Alerta!!",
    body: "La persona no ha comido!",
  }
};
admin.messaging().sendToTopic(topic, payload)
  .then(function(response) {
    console.log("Successfully sent message:", response);
    res.send('Mensaje enviado correctamente!!');
  })
  .catch(function(error) {
    console.log("Error enviando el mensaje:", error);
    res.send('Error');
  });
});
app.get('/PersonaNoBano', function (req, res) {
var topic="miCasa"
console.log(topic);
  var payload = {
  notification: {
    title: "¡¡Alerta!!",
    body: "La persona no ha ido al baño en bastante tiempo!",
  }
};
admin.messaging().sendToTopic(topic, payload)
  .then(function(response) {
    console.log("Successfully sent message:", response);
    res.send('Mensaje enviado correctamente!!');
  })
  .catch(function(error) {
    console.log("Error enviando el mensaje:", error);
    res.send('Error');
  });
});

app.get('/PersonaNoDuerme', function (req, res) {
var topic="miCasa"
console.log(topic);
  var payload = {
  notification: {
    title: "¡¡Alerta!!",
    body: "La persona no se ha ido a dormir!!!!",
  }
};
admin.messaging().sendToTopic(topic, payload)
  .then(function(response) {
    console.log("Successfully sent message:", response);
    res.send('Mensaje enviado correctamente!!');
  })
  .catch(function(error) {
    console.log("Error enviando el mensaje:", error);
    res.send('Error');
  });
});


/*
app.listen(8080, async () => {
  console.log('listening on port 8080');
});
*/

https.createServer(options, app).listen(443);