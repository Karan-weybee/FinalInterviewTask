
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
    let img= data[i].id;
    var html = `<tr>
   <th scope="row">${data[i].id}</th>
   <td><!--<img src="https://mdbcdn.b-cdn.net/img/Photos/new-templates/bootstrap-chat/ava${img%6==0?1:img%6}-bg.webp"
   alt="avatar 1" style="width: 45px; height: auto; margin-right:15px">-->${data[i].partyName}</td>
   <td><button id="${data[i].id}" class="btn btn-outline-primary" onclick="edit(id)">Edit</button></td>
   <td><button id="/${data[i].id}" class="btn btn-outline-danger" onclick="deleteParty(id)">Delete</button></td>
 </tr>`;

    document.getElementById('bodyList').insertAdjacentHTML("beforeend", html)
  }
  // document.getElementById('#partyTable').DataTable();
}

loadPartyDate();

function deleteConfirm(isDelete){
  document.querySelector(".modalPopup").classList.toggle("show-modalPopup");

  if(isDelete != 'flase'){
  console.log(isDelete)

  fetch(`https://localhost:44357/api/Parties/${isDelete}`, {
    method: "DELETE",
    headers: headers
  }).then(res => location.reload())
  }
}

function deleteParty(id) {
  id = id.slice(1, id.length)
   toggleModal(id);

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
    .then((response) => {
      if(response.status != 400){
      sessionStorage.setItem("Created", "party");}
      location.reload()
    });

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