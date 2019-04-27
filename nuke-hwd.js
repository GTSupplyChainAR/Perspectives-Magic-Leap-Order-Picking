const http = require('http');
const request = require('request');

const protocol = "http://";
const host = 'eyegaze4605api.herokuapp.com';
const standardPath = "/api/userData";

http.get(protocol + host + standardPath, (resp) => {
  var data = '';
  resp.on('data', (chunk) => {
    data += chunk;
  });

  resp.on('end', () => {
    var obj = JSON.parse(data);
    obj.forEach(e => {
      const id = e['id'];
      deleteId(id);
    });
  });
}).on("error", (err) => {
  console.log("Err: " + err.message);
});

var deleted = false;

function deleteId(id) {
  request.del(protocol + host + standardPath + "/" + id);
}