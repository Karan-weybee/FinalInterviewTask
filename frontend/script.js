function Logout(){
    localStorage.clear();
    sessionStorage.setItem("Created",'no');
    window.location.href = "/Login.html"
  }

if(sessionStorage.getItem("Created") != 'no'){
  console.log(sessionStorage.getItem("Created") +' display');
  document.getElementById(sessionStorage.getItem("Created")).style.display='block';
  sessionStorage.setItem("Created",'no');
}

function convertToIndianDateFormat(inputDate) {
  // Extract date components from the given date string
  const dateObject = new Date(inputDate);
  const day = dateObject.getDate();
  const month = dateObject.getMonth() + 1; // Months are zero-based, so we add 1
  const year = dateObject.getFullYear();

  // Create the Indian date format string
  const indianDateFormat = `${year}/${month}/${day}`;

  return indianDateFormat;
}

var modal = document.querySelector(".modalPopup");
// var trigger = document.querySelector(".trigger");
// var closeButton = document.querySelector(".close-buttonPopup");

function toggleModal(id) {
  console.log('delete pop up')
  modal.classList.toggle("show-modalPopup");
  document.getElementById('conformation').innerHTML='';
  var html = ` <button class="btnConfirn btn btn-outline-danger" onclick="deleteConfirm(${id})">Delete</button>
  <button class="btn btn-outline-secondary" onclick="deleteConfirm(false)">Cancel</button>`

  document.getElementById('conformation').insertAdjacentHTML('beforeend',html);
}


// trigger.addEventListener("click", toggleModal);
// closeButton.addEventListener("click",()=>{ toggleModal(false)});