var azure = require('azure-sb');
var querystring = require('querystring');
var http = require('http');


var connStr = "Endpoint=sb://irrigation.servicebus.windows.net/;SharedAccessKeyName=Regadores;SharedAccessKey=Pqayzkh5SsL4EUY+YIt6ekN81HS4h7VImIxXmWrzqAY=;";
if (!connStr) throw new Error('Must provide connection string');
var queueName = 'sprinklers';

console.log('Connecting to ' + connStr + ' queue ' + queueName);
var sbService = azure.createServiceBusService(connStr);
console.log('Connected');

sbService.createQueueIfNotExists(queueName, function (err) {
  if (err) {
   console.log('Failed to create queue: ', err);
  } else {
   setInterval(checkForMessages.bind(null, sbService, queueName, processMessage.bind(null, sbService)), 1000);
  }
});


function checkForMessages(sbService, queueName, callback) {
    sbService.receiveQueueMessage(queueName, { isPeekLock: false }, function (err, lockedMessage) {
      if (err) {
        if (err == 'No messages to receive') {
        //   console.log('No messages');
        } else {
          callback(err);
        }
      } else {
        callback(null, lockedMessage);
      }
    });
  }
  
  function processMessage(sbService, err, lockedMsg) {
    if (err) {
      console.log('Error on Rx: ', err);
    } else {
    //   console.log('Rx: ', lockedMsg);
      console.log(lockedMsg.body);
      var options = {
        host: "192.168.1.111",
        port: 80,
        path: '/api/pins?pinId=3&status=0',
        method: 'POST'
      };
      
      http.request(options, function(res) {
        console.log('STATUS: ' + res.statusCode);
        console.log('HEADERS: ' + JSON.stringify(res.headers));
        res.setEncoding('utf8');
        res.on('data', function (chunk) {
          console.log('BODY: ' + chunk);
        });
      }).end();
    //   sbService.deleteMessage(lockedMsg, function(err2) {
    //     if (err2) {
    //       console.log('Failed to delete message: ', err2);
    //     } else {
    //     //   console.log('Deleted message.');
    //     }
    //   })
    }
  }