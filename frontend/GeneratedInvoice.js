
var party_Id = Number(localStorage.getItem("partyId"));
const headers = new Headers();
headers.append('Authorization', `Bearer ${localStorage.getItem('tokenName')}`);
headers.append('Content-Type', 'application/json');

document.getElementById('number').innerHTML=party_Id;

async function getPartyName(){
const res = await fetch(`https://localhost:44357/api/Parties/${party_Id}?date=${localStorage.getItem("year")}/${localStorage.getItem("month")}/${localStorage.getItem("day")}`, {
    method: 'GET', // or 'POST', 'PUT', etc.
    headers:headers,
  });
    data = await res.json();
   console.log(data)
   document.getElementById('partyName').innerHTML=`<img src="https://mdbcdn.b-cdn.net/img/Photos/new-templates/bootstrap-chat/ava${party_Id%6==0?1:party_Id%6}-bg.webp"
   alt="avatar 1" style="width: 45px; height: auto; margin-right:10px">${data.partyName}`;
   }
getPartyName();

async function InvoiceProducts(){
 var date;
    const res = await fetch(`https://localhost:44357/api/invoices/${party_Id}?date=${localStorage.getItem("year")}-${localStorage.getItem("month")}-${localStorage.getItem("day")}`, {
        method: 'GET', // or 'POST', 'PUT', etc.
        headers:headers,
      });
      data = await res.json();
      console.log("----------------")
     console.log(data)
     document.getElementById('productsList').innerHTML=''
     for(let i=0;i<data.length;i++){
     var html = `
    <td><span  >${data[i].productName}</span></td>
    <td><span data-prefix>$</span><span  >${data[i].rateOfProduct}</span></td>
    <td><span data-prefix>x</span><span>${data[i].quantity}</span></td>
    <td><span data-prefix>$</span><span>${data[i].total}</span></td>
   </tr>`;
  
   document.getElementById('productsList').insertAdjacentHTML("beforeend",html);
   date=convertToIndianDateFormat(data[i].dateOfInvoice);
     }
     document.getElementById('date').innerHTML=date;
  }
  InvoiceProducts();

  async function InvoiceTotal(){
       const res = await fetch("https://localhost:44357/api/invoices", {
        method: 'GET', // or 'POST', 'PUT', etc.
        headers:headers,
      });
        data = await res.json();
       console.log(data)
    
       for(let i=0;i<data.length;i++){
    if(data[i].id==party_Id && convertToIndianDateFormat(data[i].dateOfInvoice) == `${localStorage.getItem("year")}/${localStorage.getItem("month")}/${localStorage.getItem("day")}`){
      console.log(convertToIndianDateFormat(data[i].dateOfInvoice)) 
      document.getElementById('total').innerHTML=data[i].total
    }
    
       }
       var total =Number(document.getElementById('total').innerHTML);
      var paid=Number(document.getElementById('paid').innerHTML=0);
       document.getElementById('balance').innerHTML=total-paid
    }
    
InvoiceTotal();

document.getElementById('download').addEventListener('click',
function(){
  const element = document.querySelector('#InvoiceGeneration'); // Choose the parent element that wraps your invoice content
  html2pdf(element);
}
// function(){
//   window.print();
// }
);
document.getElementById('back').addEventListener('click',
function(){
  window.location.href = "/Invoice.html"
});