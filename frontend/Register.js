document.addEventListener('DOMContentLoaded', function () {
  
    var form = document.getElementById('RegisterUser');

    // Attach an event listener to the form for the submit event
    form.addEventListener('submit', function (event) {
        event.preventDefault();
      
        var userEmail = document.getElementById('userEmail').value;
        var password =  document.getElementById('password').value;
        var repeatPass =document.getElementById('repeatpassword').value;
        
    if(password==repeatPass){
        console.log(password)
    RegisterUser(userEmail,password);
    }
})
});

async function RegisterUser(userEmail,password){
 var token;
    var data = await fetch("https://localhost:44357/api/Users", {
        method: "POST",
        body: JSON.stringify({
            email: userEmail,
            password: password
        }),
        headers: {
            "Content-type": "application/json; charset=UTF-8"
        }
    })
        .then((response) => {
            return response.json();
        }).then(data => token = data);

    localStorage.setItem("tokenName",token.tokenName);
    window.location.href = "/index.html"

}