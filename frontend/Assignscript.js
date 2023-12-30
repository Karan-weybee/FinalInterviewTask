
var data;
var AssignId;
const headers = new Headers();
headers.append('Authorization', `Bearer ${localStorage.getItem('tokenName')}`);
headers.append('Content-Type', 'application/json');

async function loadAssignPartyDate(){
   const res = await fetch("https://localhost:44357/api/AssignParties", {
    method: 'GET', // or 'POST', 'PUT', etc.
    headers:headers,
  });
  if(res.status==401){
    window.location.href = "/Register.html"
  }
    data = await res.json();
   console.log(data)

   for(let i=0;i<data.length;i++){
   var html = `<tr>
   <th scope="row">${data[i].id}</th>
   <td>${data[i].partyName}</td>
   <td>${data[i].productName}</td>
   <td><button id="${data[i].id}" class="btn btn-outline-primary" onclick="edit(id)">Edit</button></td>
   <td><button id="/${data[i].id}" class="btn btn-outline-danger"  onclick="deleteAssign(id)">Delete</button></td>
 </tr>`;

 document.getElementById('bodyList').insertAdjacentHTML("beforeend",html)
   }
fillPartyData('selectParty');
fillProductData('selectProduct');
  
}

loadAssignPartyDate();

async function fillPartyData(selectParty){
  var partyName;
  var partyId;
  if(AssignId != null){
    AssignId=Number(AssignId);
    const Assign = await fetch(`https://localhost:44357/api/AssignParties/${AssignId}`, {
      method: 'GET', // or 'POST', 'PUT', etc.
      headers:headers,
    });
   var Assigndata = await Assign.json();
   partyName=Assigndata.partyName;
   console.log("--------------"+partyName)
  }
    const partyres = await fetch("https://localhost:44357/api/Parties", {
      method: 'GET', // or 'POST', 'PUT', etc.
      headers:headers,
    });
    Partydata = await partyres.json();
   console.log(Partydata)

   if(AssignId != null){
    for(let i=0;i<Partydata.length;i++){
      if(partyName==Partydata[i].partyName){
        partyId=Partydata[i].id;
      }
    }
    
    document.getElementById(selectParty).innerHTML='';
    var html1=` <option value="${partyId}">${partyName}</option>`;
    document.getElementById(selectParty).insertAdjacentHTML("beforeend",html1)
   }
   for(let i=0;i<Partydata.length;i++){
    var html1=` <option value="${Partydata[i].id}">${Partydata[i].partyName}</option>`;
    document.getElementById(selectParty).insertAdjacentHTML("beforeend",html1)
   }

}
async function fillProductData(selectProduct){
  var productName;
  var productId;
  if(AssignId != null){
    AssignId=Number(AssignId);
    const Assign = await fetch(`https://localhost:44357/api/AssignParties/${AssignId}`, {
      method: 'GET', // or 'POST', 'PUT', etc.
      headers:headers,
    });
   var Assigndata = await Assign.json();
   productName=Assigndata.productName;
   console.log("--------------"+productName)
  }

    const productres = await fetch("https://localhost:44357/api/Products", {
      method: 'GET', // or 'POST', 'PUT', etc.
      headers:headers,
    });
    var Productdata = await productres.json();
   console.log(Productdata)

   if(AssignId != null){
    for(let i=0;i<Productdata.length;i++){
      if(productName==Productdata[i].productName){
        productId=Productdata[i].id;
      }
    }
    
    document.getElementById(selectProduct).innerHTML='';
    var html1=` <option value="${productId}">${productName}</option>`;
    document.getElementById(selectProduct).insertAdjacentHTML("beforeend",html1)
   }
   for(let i=0;i<Productdata.length;i++){

    var html1=` <option value="${Productdata[i].id}">${Productdata[i].productName}</option>`;
    document.getElementById(selectProduct).insertAdjacentHTML("beforeend",html1)
   }
}

function deleteConfirm(isDelete){
  modal.classList.toggle("show-modalPopup");

  if(isDelete != 'flase'){
  console.log(isDelete)

  fetch(`https://localhost:44357/api/AssignParties/${isDelete}`, {
    method: "DELETE",
    headers: headers

  }).then(res=>location.reload());
  }
}

function deleteAssign(id){
  id = id.slice(1, id.length)
  toggleModal(id);

   
}

function createAssign(){
  var Party = Number(document.getElementById('selectParty').value);
  var Product = Number(document.getElementById('selectProduct').value);

  fetch("https://localhost:44357/api/AssignParties", {
  method: "POST",
  body: JSON.stringify({
    PartyId: Party,
    ProductId:Product
  }),
      headers:headers 
})
  .then((response) => {
    if(response.status != 400){
    sessionStorage.setItem("Created", "assign");}
    location.reload()
  });
}

function closeModel(){
  console.log('close')
  $('.fade').hide();
}

async function edit(id){
  console.log(id)
  AssignId=id
  document.getElementById('exampleModal').style.display='block';
  
fillPartyData('selectParty2');
fillProductData('selectProduct2');

}
function editFromModel(){

    var Party = Number(document.getElementById('selectParty2').value);
    var Product = Number(document.getElementById('selectProduct2').value);

fetch(`https://localhost:44357/api/AssignParties/${AssignId}`, {
  method: "PUT",
  body: JSON.stringify({
    PartyId: Party,
    ProductId:Product
  }),
  headers: headers
})
  .then((response) => location.reload());

}