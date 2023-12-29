
var data;
var party_Id;
const headers = new Headers();
headers.append('Authorization', `Bearer ${localStorage.getItem('tokenName')}`);
headers.append('Content-Type', 'application/json');

async function loadInvoiceData() {
  fillPartyData('selectParty');
}

loadInvoiceData();

async function fillPartyData(selectParty) {
  const partyres = await fetch("https://localhost:44357/api/Parties", {
    method: 'GET', // or 'POST', 'PUT', etc.
    headers: headers,
  });
  Partydata = await partyres.json();
  console.log(Partydata)

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
  
  // document.getElementById('field2').innerHTML = '';
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

  fetch(`https://localhost:44357/api/invoices?partyId=${party_Id}&productId=${id}`, {
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
    var date = new Date();
  }
  else {
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

async function edit(id) {
  console.log(id)
  party_Id = id
  document.getElementById('exampleModal').style.display = 'block';
 
  fillInvoiceProducts('');
  $('#InvoiceProduct').DataTable({ searching: false });

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
  if (products == '') {
    res = await fetch(`https://localhost:44357/api/invoices/${party_Id}`, {
      method: 'GET', // or 'POST', 'PUT', etc.
      headers: headers,
    });
    data = await res.json();
  }
  else {
    for (let i = 0; i < products.length; i++) {
      res = await fetch(`https://localhost:44357/api/Invoices/Search?partyId=${party_Id}&productName=${products[i]}`, {
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

  }
  document.getElementById('ProductList').innerHTML = ''

  console.log("----------------")
  console.log(data)

  for (let i = 0; i < data.length; i++) {
    var html = `<tr>
   <th scope="row">${data[i].id}</th>
   <td>${data[i].productName}</td>
   <td> <input type="number" placeholder="rate" style="width: 61px; margin-left: 10px;
   border: none;" id="rate${data[i].id}" min="0" value="${data[i].rateOfProduct}" readonly/></td>
   <td>  <input type="number" placeholder="Quantity" style="width: 60px; margin-left: 10px;" 
        id="quantity${data[i].id}" min="1" value="${data[i].quantity}" oninput="updateInvoiceProduct(id)"/></td>
   <td> <input type="text" id="date${data[i].id}" style="width: 7vw; margin-left: 10px;
   border: none;" value="${data[i].dateOfInvoice}"></td>
   <td>${data[i].total}</td>
   <!--<td><button id="${data[i].id}" class="btn btn-outline-primary" onclick="updateInvoiceProduct(id)">Update</button></td>-->
   <td><button id="${data[i].id}" class="btn btn-outline-danger" onclick="deleteProduct(id)">delete</button></td>
 </tr>`;

    document.getElementById('ProductList').insertAdjacentHTML("beforeend", html)
  }
  // $('#InvoiceProduct').DataTable({searching: false});
}

function updateInvoiceProduct(quantity) {
  let productId = quantity.slice(8, quantity.length);
  var rateOfProduct = Number(document.getElementById(`rate${productId}`).value);
  var quantity = Number(document.getElementById(`quantity${productId}`).value);
  var date = document.getElementById(`date${productId}`).value;
  party_Id = Number(party_Id);
  productId = Number(productId)
  console.log("quantity :- " + quantity)
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

function sortId() {
  console.log("id sort")
}
function view(partyid) {
  localStorage.setItem("partyId", partyid);
  window.location.href = "/GenerateInvoice.html";
}

function create() {
  window.location.href = "/CreateInvoice.html";
}