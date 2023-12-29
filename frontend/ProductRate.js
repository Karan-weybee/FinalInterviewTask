
var data;
var ProductRateId;
const headers = new Headers();
headers.append('Authorization', `Bearer ${localStorage.getItem('tokenName')}`);
headers.append('Content-Type', 'application/json');

async function loadProductaRateData(){
   const res = await fetch("https://localhost:44357/api/ProductRates", {
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
   <td>${data[i].rate}</td>
   <td>${data[i].dateOfRate}</td>
   <td><button id="${data[i].id}" class="btn btn-outline-primary" onclick="edit(id)">Edit</button></td>
   <td><button id="/${data[i].id}" class="btn btn-outline-danger"  onclick="deleteProductRate(id)">Delete</button></td>
 </tr>`;

 document.getElementById('bodyList').insertAdjacentHTML("beforeend",html)
   }

fillProductData('selectProduct');
  
}

loadProductaRateData();

async function fillPartyData(selectParty){
    const partyres = await fetch("https://localhost:44357/api/Parties", {
      method: 'GET', // or 'POST', 'PUT', etc.
      headers:headers,
    });
    Partydata = await partyres.json();
   console.log(Partydata)

   for(let i=0;i<Partydata.length;i++){

    var html1=` <option value="${Partydata[i].id}">${Partydata[i].partyName}</option>`;
    document.getElementById(selectParty).insertAdjacentHTML("beforeend",html1)
   }
}
async function fillProductData(selectProduct){
  var productName;
  var productId;
  if(ProductRateId != null){
    ProductRateId=Number(ProductRateId);
    const Assign = await fetch(`https://localhost:44357/api/ProductRates/${ProductRateId}`, {
      method: 'GET', // or 'POST', 'PUT', etc.
      headers:headers,
    });
   var prRatedata = await Assign.json();
   productName=prRatedata.productName;
   console.log("--------------"+productName)
  }

    const productres = await fetch("https://localhost:44357/api/Products", {
      method: 'GET', // or 'POST', 'PUT', etc.
      headers:headers,
    });
   var Productdata = await productres.json();
   console.log(Productdata)

   if(ProductRateId != null){
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

function deleteProductRate(id){
    id=id.slice(1,id.length)
    console.log(id)
   
    fetch(`https://localhost:44357/api/ProductRates/${id}`, {
      method: "DELETE",
      headers: headers
    })
     .then(x=>location.reload())
    
}

function createProductRate(){
  var Product = Number(document.getElementById('selectProduct').value);
  var rateOfProduct = Number(document.getElementById('rate').value);
  var date = document.getElementById('date').value;

  console.log(Product +" "+typeof rateOfProduct +" "+typeof date)
  fetch("https://localhost:44357/api/ProductRates", {
  method: "POST",
  body: JSON.stringify({
    ProductId:Product,
    rate:rateOfProduct,
    dateOfRate:date
  }),
  headers: headers
})
  .then((response) => location.reload());
  console.log("Assigned")
}

function closeModel(){
  console.log('close')
  $('.fade').hide();
}

async function edit(id){
  console.log(id)
  ProductRateId=id
  document.getElementById('exampleModal').style.display='block';
fillProductData('selectProduct2');
const res = await fetch(`https://localhost:44357/api/ProductRates/${id}`, {
  method: 'GET', // or 'POST', 'PUT', etc.
  headers:headers,
});
data = await res.json();
console.log(data)
document.getElementById('rate2').value=data.rate;
}

function editFromModel(){

    var Product = Number(document.getElementById('selectProduct2').value);
    var rateOfProduct=Number(document.getElementById('rate2').value);
    var date = document.getElementById('date2').value;
fetch(`https://localhost:44357/api/ProductRates/${ProductRateId}`, {
  method: "PUT",
  body: JSON.stringify({
    ProductId:Product,
    rate:rateOfProduct,
    dateOfRate:date
  }),
  headers: headers
})
  .then((response) =>location.reload());
}