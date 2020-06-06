'use strict';

const socket = io();

const salidaHumano = document.querySelector('.salida-humano');
const salidaBot = document.querySelector('.salida-bot');

const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
const recognition = new SpeechRecognition();
var hasSpoken=false;
recognition.lang = 'es-ES';
recognition.interimResults = false;
recognition.maxAlternatives = 1;


async function init() {
  hasSpoken=false;
  recognition.start();
  await sleep(10000);
  recognition.stop();
  if(hasSpoken==false){
  const synth = window.speechSynthesis;
  const ut = new SpeechSynthesisUtterance("Como no se ha obtenido respuesta, se procede a llamar a la ambulancia.");
  synth.speak(ut);
  }
}

function sleep(ms) {
  return new Promise((resolve) => {
    setTimeout(resolve, ms);
  });
}   

window.onload = function() {
    const synth = window.speechSynthesis;
    const ut = new SpeechSynthesisUtterance("El sistema ha detectado que te has caido, por favor , ¿podrías confirmarlo?");
    synth.speak(ut);
    ut.onend=function(event) {
      init();
    }
    
  }
  
recognition.addEventListener('speechstart', () => {
  console.log('Voz detectada.');
});

recognition.addEventListener('result', (e) => {
  hasSpoken=true;
  let last = e.results.length - 1;
  let text = e.results[last][0].transcript;
  salidaHumano.textContent = text;

  socket.emit('mensaje', text);
});

recognition.addEventListener('error', (e) => {
  salidaBot.textContent = 'Error: ' + e.error;
});

function synthVoice(text) {
  const synth = window.speechSynthesis;
  const utterance = new SpeechSynthesisUtterance();
  utterance.text = text;
  synth.speak(utterance);
  if(text=="Simplemente, responda Sí o No.")
  utterance.onend= function (event){
      init();
  }
  
}

socket.on('bot respuesta', function(replyText) {
  synthVoice(replyText);

  if(replyText == '') replyText = '(Sin respuesta)';
  salidaBot.textContent = replyText;
});