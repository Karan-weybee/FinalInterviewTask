
var data;
var productId;
const headers = new Headers();
headers.append('Authorization', `Bearer ${localStorage.getItem('tokenName')}`);
headers.append('Content-Type', 'application/json');

async function loadproductDate(){
   const res = await fetch("https://localhost:44357/api/Products", {
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
   <td>${data[i].productName}</td>
   <td><button id="${data[i].id}" class=" btn btn-outline-primary" onclick="edit(id)">Edit</button></td>
   <td><button id="/${data[i].id}" class=" btn btn-outline-danger"  onclick="deleteproduct(id)">Delete</button></td>
 </tr>`;

 document.getElementById('bodyList').insertAdjacentHTML("beforeend",html)
   }
}

loadproductDate();

function deleteConfirm(isDelete){
  document.querySelector(".modalPopup").classList.toggle("show-modalPopup");

  if(isDelete != 'flase'){
  console.log(isDelete)

  fetch(`https://localhost:44357/api/Products/${isDelete}`, {
      method: "DELETE",
      headers: headers
    }).then(res=>location.reload())
  }
}

function deleteproduct(id){
  id = id.slice(1, id.length)
  toggleModal(id);
   
   
     
}

function createproduct(){
  var productname = document.getElementById('productName').value;
  console.log(productname)

  fetch("https://localhost:44357/api/Products", {
  method: "POST",
  body: JSON.stringify({
    productName: productname
  }),
  headers:headers
})
  .then((response) =>{
    if(response.status != 400){
    sessionStorage.setItem("Created", "product");}
    location.reload()
  });
}

function closeModel(){
  console.log('close')
  $('.fade').hide();
}

async function edit(id){
  console.log(id)
  productId=id
  document.getElementById('exampleModal').style.display='block';
  
const res = await fetch(`https://localhost:44357/api/Products/${id}`, {
  method: 'GET', // or 'POST', 'PUT', etc.
  headers:headers,
});
data = await res.json();
console.log(data)

document.getElementById('productNameEdit').value=data.productName;

}
function editFromModel(){
console.log("product id "+ productId)
var name= document.getElementById('productNameEdit').value;
fetch(`https://localhost:44357/api/Products/${productId}`, {
  method: "PUT",
  body: JSON.stringify({
    productName: name
  }),
  headers: headers
})
  .then((response) => response.json())
  .then((json) => location.reload());

}