
var data;
var party_Id;
var year,month,day;
const headers = new Headers();
headers.append('Authorization', `Bearer ${localStorage.getItem('tokenName')}`);
headers.append('Content-Type', 'application/json');

async function loadInvoiceData() {
  const res = await fetch("https://localhost:44357/api/invoices", {
    method: 'GET', // or 'POST', 'PUT', etc.
    headers:headers,
  });
  if(res.status==401){
    window.location.href = "/Register.html"
  }
    data = await res.json();
   console.log(data)

   loadInvoiceList(data);
  fillPartyData('selectParty');
  fillPartyData('selectPartySearch');
  
}

loadInvoiceData();

async function RangeInvoice(){
  var startDate = document.getElementById('StartDate').value;
  var endDate = document.getElementById('EndDate').value;

  const res = await fetch(`https://localhost:44357/api/invoices/Range?StartDate=${startDate}&EndDate=${endDate}`, {
    method: 'GET', // or 'POST', 'PUT', etc.
    headers:headers,
  });
  if(res.status==401){
    window.location.href = "/Register.html"
  }
    data = await res.json();
   console.log(data)
   loadInvoiceList(data);
}

function loadInvoiceList(data){
  document.getElementById('bodyList').innerHTML='';
  for(let i=0;i<data.length;i++){
    let img=data[i].id;
    var d = new Date(data[i].dateOfInvoice);
    var html = `<tr>
    <th>${data[i].partyId}</th>
    <td style="display: none;">${data[i].id}</td>
    <td><img src="https://mdbcdn.b-cdn.net/img/Photos/new-templates/bootstrap-chat/ava${img%6==0?1:img%6}-bg.webp"
    alt="avatar 1" style="width: 45px; height: auto; margin-right:15px"> 
    ${data[i].partyName}</td>
    <td>${convertToIndianDateFormat(data[i].dateOfInvoice)}</td>
    <td>${data[i].total}</td>
    <td><button id="${data[i].id}" class=" btn btn-outline-primary" onclick="edit(id,${d.getFullYear()},${d.getMonth()+1},${d.getDate()})">Edit</button></td>
    <td><button class="btn btn-outline-secondary" id="${data[i].id}" onclick="view(id,${d.getFullYear()},${d.getMonth()+1},${d.getDate()})">View</button></td>
  </tr>`;
 
 document.getElementById('bodyList').insertAdjacentHTML("beforeend",html)
    }
    $("#invoiceList").DataTable();
}
async function fillPartyData(selectParty) {
  try {
    const partyres = await fetch("https://localhost:44357/api/Parties", {
      method: 'GET', // or 'POST', 'PUT', etc.
      headers: headers,
    });

    Partydata = await partyres.json();
    console.log(Partydata)
  } catch (error) {
    window.location.href = "/Register.html"
  }
  for (let i = 0; i < Partydata.length; i++) {

    var html1 = ` <option value="${Partydata[i].id}">${Partydata[i].partyName}</option>`;
    document.getElementById(selectParty).insertAdjacentHTML("beforeend", html1)
  }
}

async function fillProductData(selectParty, selectProduct) {

  var PartyId = Number(document.getElementById(selectParty).value);
  const res = await fetch(`https://localhost:44357/api/Parties/${PartyId}`, {
    method: 'GET', // or 'POST', 'PUT', etc.
    headers: headers,
  });
  data = await res.json();
  console.log(data.partyName)

  const Assignres = await fetch("https://localhost:44357/api/AssignParties", {
    method: 'GET', // or 'POST', 'PUT', etc.
    headers: headers,
  });
  var Assigndata = await Assignres.json();


  const Products = Assigndata.filter((element) => element.partyName == data.partyName);
  var productNames = [];
  var productId = [];
  for (let i = 0; i < Products.length; i++) {
    productNames.push(Products[i].productName)
  }
  console.log(productNames)

  const Productres = await fetch("https://localhost:44357/api/Products", {
    method: 'GET', // or 'POST', 'PUT', etc.
    headers: headers,
  });
  var Productdata = await Productres.json();

  var filterProducts = [];
  for (let i = 0; i < productNames.length; i++) {

    filterProducts.push(Productdata.find((element) => element.productName == productNames[i]));

  }
  for (let i = 0; i < filterProducts.length; i++) {
    productId.push(filterProducts[i].id)
  }
  console.log(productId)
  document.getElementById(selectProduct).innerHTML = '';
  var html2 = ` <option >Select Products</option>`;
  document.getElementById(selectProduct).insertAdjacentHTML("beforeend", html2)

  for (let i = 0; i < productId.length; i++) {

    var html1 = ` <option value="${productId[i]}">${productNames[i]}</option>`;
    document.getElementById(selectProduct).insertAdjacentHTML("beforeend", html1);


  }
}

async function fillRate(selectProduct, rate) {
  var ProductId = Number(document.getElementById(selectProduct).value);
  console.log(ProductId)
  const res = await fetch(`https://localhost:44357/api/Products/${ProductId}`, {
    method: 'GET', // or 'POST', 'PUT', etc.
    headers: headers,
  });
  data = await res.json();
  console.log(data)

  const ProductRatesres = await fetch(`https://localhost:44357/api/ProductRates`, {
    method: 'GET', // or 'POST', 'PUT', etc.
    headers: headers,
  });
  var ProductRatedata = await ProductRatesres.json();
  var product = ProductRatedata.findLast((element) => element.productName == data.productName);
  console.log(ProductRatedata)
  document.getElementById(rate).value = product.rate;
}

function deleteProduct(id) {

  id = Number(id)
  console.log(typeof id)

  fetch(`https://localhost:44357/api/invoices?partyId=${party_Id}&productId=${id}&date=${year}-${month}-${day}`, {
    method: "DELETE",
    headers: headers
  }).then(req => fillInvoiceProducts(''))

}

function createInvoice() {
  if (party_Id != null) {
    var Party = Number(document.getElementById('selectPartyInModel').value);
    var Product = Number(document.getElementById('selectProductInModel').value);
    var rateOfProduct = Number(document.getElementById('rateInModel').value);
    var quantity = Number(document.getElementById('quantityInModel').value);
    if(month.toString().length < 2){
      month = `0${month}`
    }
    if(day.toString().length<2){
      day=`0${day}`
    }
   
    var date = `${year}-${month}-${day}`;
  }
  else {
    console.log("hii")
    var Party = Number(document.getElementById('selectParty').value);
    var Product = Number(document.getElementById('selectProduct').value);
    var rateOfProduct = Number(document.getElementById('rate').value);
    var quantity = Number(document.getElementById('quantity').value);
    var date = new Date();
  }

  fetch("https://localhost:44357/api/invoices", {
    method: "POST",
    body: JSON.stringify({
      PartyId: Party,
      ProductId: Product,
      RateOfProduct: rateOfProduct,
      Quantity: quantity,
      DateOfInvoice: date
    }),
    headers: headers
  })
    .then((response) => {
      if(response.status != 400){
      sessionStorage.setItem("Created", "invoice");}
    
      if (party_Id != null) {
        fillInvoiceProducts('');
      }
      else { window.location.href = "/Invoice.html" }
    });
  console.log("created")

}


function closeModel() {
  console.log('close')
  $('.fade').hide();
  party_Id = null;
  location.reload();
}

async function edit(id,years,months,days) {
  console.log(id)
  party_Id = id;
  year = years;
  month=months;
  day=days
  console.log(year)
  document.getElementById('exampleModal').style.display = 'block';

  fillInvoiceProducts('');
  $('#InvoiceProduct').DataTable({ searching: false,paging:false});

  document.getElementById('selectPartyInModel').innerHTML = ''
  document.getElementById('selectPartyInModel').innerHTML = "<option value=''>Select Party</option>";
  const partyres = await fetch(`https://localhost:44357/api/Parties/${party_Id}`, {
    method: 'GET', // or 'POST', 'PUT', etc.
    headers: headers,
  });
  Partydata = await partyres.json();
  console.log(Partydata)

  var html1 = ` <option value="${Partydata.id}">${Partydata.partyName}</option>`;
  document.getElementById('selectPartyInModel').insertAdjacentHTML("beforeend", html1)

 
}


async function fillInvoiceProducts(products) {
  let res
  var details = [];
  details.length = products.length;
  var link = `https://localhost:44357/api/invoices/${party_Id}?date=${year}/${month}/${day}`
  console.log(link)
  if (products == '') {
    res = await fetch(`https://localhost:44357/api/invoices/${party_Id}?date=${year}/${month}/${day}`, {
      method: 'GET', // or 'POST', 'PUT', etc.
      headers: headers,
    });
    data = await res.json();
    fillModelData(data);

  }
  else {
    for (let i = 0; i < products.length; i++) {
      res = await fetch(`https://localhost:44357/api/Invoices/Search?partyId=${party_Id}&productName=${products[i]}&date=${year}/${month}/${day}`, {
        method: 'GET', // or 'POST', 'PUT', etc.
        headers: headers,
      });
      details[i] = await res.json();

    }
    data = []
    for (let i = 0; i < details.length; i++) {
      data.push(details[i][0]);
    }
    console.log(data)
    fillModelData(data);
  }
 

 
  
  // $('#InvoiceProduct').DataTable({searching: false});
}

function fillModelData(data) {
  var size=Number(document.getElementById('Size').value);
  var index= data.length>size?size:data.length;
  var updatedSize=[];
  for(let i=0;i<data.length;i++){
      if(data[i]){
        updatedSize.push(data[i]);
      }
  }

  data=updatedSize;
  console.log(data)
  document.getElementById('ProductList').innerHTML = ''
  for (let i = 0; i < index; i++) {
    if(data[i]){
    
    var html = `<tr>
   <th scope="row">${data[i].id}</th>
   <td>${data[i].productName}</td>
   <td> <input type="number" placeholder="rate" style="width: 61px; margin-left: 10px;
   border: none;" id="rate${data[i].id}" min="0" value="${data[i].rateOfProduct}" readonly/></td>
   <td>  <input type="number" placeholder="Quantity" style="width: 60px; margin-left: 10px;" 
        id="quantity${data[i].id}" min="1" value="${data[i].quantity}" oninput="updateInvoiceProduct(id)"/></td>
   <td> <input type="text" id="date${data[i].id}" style="width: 8.5vw; margin-left: 10px;
   border: none;" value="${convertToIndianDateFormat(data[i].dateOfInvoice)}"></td>
   <td>${data[i].total}</td>
   <!--<td><button id="${data[i].id}" class="btn btn-outline-primary" onclick="updateInvoiceProduct(id)">Update</button></td>-->
   <td><button id="${data[i].id}" class="btn btn-outline-danger" onclick="deleteProduct(id)">Delete</button></td>
 </tr>`;
    
    document.getElementById('ProductList').insertAdjacentHTML("beforeend", html)
    }
  }
}
function updateInvoiceProduct(quantity) {
  var productId = quantity.slice(8, quantity.length);
  var rateOfProduct = Number(document.getElementById(`rate${productId}`).value);
  var quantity = Number(document.getElementById(`quantity${productId}`).value);
  if(month.toString().length < 2){
    month = `0${month}`
  }
  if(day.toString().length<2){
    day=`0${day}`
  }
  var date = `${year}-${month}-${day}`;
  party_Id = Number(party_Id);
  productId = Number(productId)
  console.log("quantity :- " + productId)
  fetch(`https://localhost:44357/api/invoices/${party_Id}`, {
    method: "PUT",
    body: JSON.stringify({
      id: productId,
      RateOfProduct: rateOfProduct,
      quantity: quantity,
      dateOfInvoice: date
    }),
    headers: headers
  }).then(req => fillInvoiceProducts(''));

}
function searchProduct(selectedOptions) {
  console.log(selectedOptions);
  fillInvoiceProducts(selectedOptions);

}

async function sortId() {

  var dataset = document.getElementById('sortId').getAttribute('dataSet');

  res = await fetch(`https://localhost:44357/api/Invoices/Sort?partyId=${party_Id}&toggle=${dataset}&sortField=Id&date=${year}/${month}/${day}`, {
    method: 'GET', // or 'POST', 'PUT', etc.
    headers: headers,
  });
  var data = await res.json();
  fillModelData(data);
 console.log(data)
  dataset=="0"? document.getElementById('sortId').setAttribute('dataSet', 1): document.getElementById('sortId').setAttribute('dataSet', 0);
  
}
async function sortProductName() {
  console.log("pr name sort")
 
  var dataset = document.getElementById('sortProductName').getAttribute('dataSet');

  res = await fetch(`https://localhost:44357/api/Invoices/Sort?partyId=${party_Id}&toggle=${dataset}&sortField=ProductName&date=${year}/${month}/${day}`, {
    method: 'GET', // or 'POST', 'PUT', etc.
    headers: headers,
  });
  var data = await res.json();
  fillModelData(data);

  dataset=="0"? document.getElementById('sortProductName').setAttribute('dataSet', 1): document.getElementById('sortProductName').setAttribute('dataSet', 0);
  
}
function view(partyid,years,months,days) {
  localStorage.setItem("partyId", partyid);
  localStorage.setItem("year", years);
  localStorage.setItem("month", months);
  localStorage.setItem("day", days);
  window.location.href = "/GenerateInvoice.html";
}

function create() {
  window.location.href = "/CreateInvoice.html";
}
async function nextPage(){
  var size=Number(document.getElementById('Size').value);
  console.log(size)
  console.log("next")
  var currentPage = Number(document.getElementById('indexPage').innerHTML);
  currentPage++;
  res = await fetch(`https://localhost:44357/api/Invoices/Page/${party_Id}?size=${size}&pageIndex=${currentPage}&date=${year}-${month}-${day}`, {
    method: 'GET', // or 'POST', 'PUT', etc.
    headers: headers,
  });
  var data = await res.json();
  fillModelData(data);

  document.getElementById('indexPage').innerHTML=currentPage;
}
async function prevPage(){
  var size=Number(document.getElementById('Size').value);
  console.log("prev")
  var currentPage = Number(document.getElementById('indexPage').innerHTML);
  currentPage--;
  res = await fetch(`https://localhost:44357/api/Invoices/Page/${party_Id}?size=${size}&pageIndex=${currentPage}&date=${year}-${month}-${day}`, {
    method: 'GET', // or 'POST', 'PUT', etc.
    headers: headers,
  });
  var data = await res.json();
  fillModelData(data);

  document.getElementById('indexPage').innerHTML=currentPage;
}
function changeSize(){
  selecteProducts();
}
function Clear(){
  location.reload();
}