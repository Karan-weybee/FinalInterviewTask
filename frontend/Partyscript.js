
var data;
var PartyId;
const headers = new Headers();
headers.append('Authorization', `Bearer ${localStorage.getItem('tokenName')}`);
headers.append('Content-Type', 'application/json');

async function loadPartyDate() {
  
  const res = await fetch("https://localhost:44357/api/Parties", {
    method: 'GET', // or 'POST', 'PUT', etc.
    headers:headers,
  });
  if(res.status==401){
    window.location.href = "/Register.html"
  }
  data = await res.json();
  
  console.log(data)

  for (let i = 0; i < data.length; i++) {
    var html = `<tr>
   <th scope="row">${data[i].id}</th>
   <td>${data[i].partyName}</td>
   <td><button id="${data[i].id}" class="btn btn-outline-primary" onclick="edit(id)">Edit</button></td>
   <td><button id="/${data[i].id}" class="btn btn-outline-danger" onclick="deleteParty(id)">Delete</button></td>
 </tr>`;

    document.getElementById('bodyList').insertAdjacentHTML("beforeend", html)
  }
}

loadPartyDate();


function deleteParty(id) {
  id = id.slice(1, id.length)
  console.log(id)

  fetch(`https://localhost:44357/api/Parties/${id}`, {
    method: "DELETE",
    headers: headers
  }).then(res => location.reload())

}

function createParty() {
  var Partyname = document.getElementById('partyName').value;
  console.log(Partyname)
  
  fetch("https://localhost:44357/api/Parties", {
    method: "POST",
    body: JSON.stringify({
      partyName: Partyname
    }),
    headers: headers
  })
    .then((response) => location.reload());

}

function closeModel() {
  console.log('close')
  $('.fade').hide();
}

async function edit(id) {
  console.log(id)
  PartyId = id
  document.getElementById('exampleModal').style.display = 'block';
 

  const res = await fetch(`https://localhost:44357/api/Parties/${id}`,{
    method: 'GET', // or 'POST', 'PUT', etc.
    headers:headers,
  });
  data = await res.json();
  console.log(data)

  document.getElementById('partyNameEdit').value = data.partyName;

}
function editFromModel() {
  console.log("party id " + PartyId)
  var name = document.getElementById('partyNameEdit').value;
  fetch(`https://localhost:44357/api/Parties/${PartyId}`, {
    method: "PUT",
    body: JSON.stringify({
      partyName: name
    }),
    headers: headers
  })
    .then((response) => response.json())
    .then((json) => location.reload());
  
}