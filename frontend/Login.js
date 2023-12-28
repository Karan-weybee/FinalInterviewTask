document.addEventListener('DOMContentLoaded', function () {
  
    var form = document.getElementById('LoginUser');

    // Attach an event listener to the form for the submit event
    form.addEventListener('submit', function (event) {
        event.preventDefault();
      
        var email = document.getElementById('userEmail').value;
        var pass = document.getElementById('password').value;
        
        signin(email,pass);
    
})
});

async function signin(email,pass) {
  
    var token;
    var data = await fetch("https://localhost:44357/api/Users/Login", {
        method: "POST",
        body: JSON.stringify({
            email: email,
            password: pass
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