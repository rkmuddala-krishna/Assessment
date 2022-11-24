const listtallotpuri = 'api/otp/listall';
const generateotpuri = 'api/otp/GenerateOTP';
const validateotpuri = 'api/otp/ValidateOTP';
let todos = [];

function getItems() {
  fetch(listtallotpuri)
    .then(response => response.json())
    .then(data => _displayItems(data))
    .catch(error => console.error('Unable to get items.', error));
}

function _displayItems(data) {
  const tBody = document.getElementById('todos');
  tBody.innerHTML = '';

  data.forEach(item => {


    let tr = tBody.insertRow();

    let td1 = tr.insertCell(0);
    td1.innerHTML = item.otpCode;

    let td2 = tr.insertCell(1);
    td2.innerHTML = item.email;

    let td3 = tr.insertCell(2);
    td3.innerHTML = item.createdDate;
    let td4 = tr.insertCell(3);
    td4.innerHTML = item.expired;
    let td5 = tr.insertCell(4);
    td5.innerHTML = item.currentdate;

  });

  todos = data;
}

function displaygeneratedotp(response, email) {
  document.getElementById('response').style.visibility = 'visible'
  document.getElementById('errorresponse').style.visibility = 'hidden'

  const otpcodeLabel = document.getElementById('otpcode');
  const emailsentstatusLabel = document.getElementById('emailsentstatus');

  otpcodeLabel.innerText = ' Your code : ' + response.data.otp + ' is valid for 60 Seconds'
  emailsentstatusLabel.innerText = 'Code sent to email_address : ' + email + 'is : ' + response.data.emailsentstatus == "0" ? "successful" : "Failed";

}

function generated_otperror(error) {
  document.getElementById('errorresponse').style.visibility = 'visible'
  const otperrorLabel = document.getElementById('otperror');
  otperrorLabel.innerText = error.statusText;

  document.getElementById('response').style.visibility = 'hidden'

  const otpcodeLabel = document.getElementById('otpcode');
  const emailsentstatusLabel = document.getElementById('emailsentstatus');

  otpcodeLabel.innerText = ''
  emailsentstatusLabel.innerText = '';

}

function validated_otperror(error) {
  document.getElementById('errorresponse').style.visibility = 'visible'
  const otperrorLabel = document.getElementById('otperror');
  otperrorLabel.innerText = error.statusText;

  document.getElementById('response').style.visibility = 'hidden'

  const otpcodestatusLabel = document.getElementById('otpcodestatus');
  const emailaddressLabel = document.getElementById('emailaddress');
  otpcodestatusLabel.innerText = '';
  emailaddressLabel.innerText = '';

}
function displayvalidatedotp(response) {
  document.getElementById('response').style.visibility = 'visible'
  document.getElementById('errorresponse').style.visibility = 'hidden'

  const otpcodestatusLabel = document.getElementById('otpcodestatus');
  const emailaddressLabel = document.getElementById('emailaddress');
  const otpcodecreateddateLabel = document.getElementById('otpcodecreateddate');
  otpcodestatusLabel.innerText = 'OTP Code status :' + response.data.otP_Status + ' OTP created at:' + response.data.otp_createddate //+ ' and expires:' + timeDifference(new Date(), new Date(response.data.otp_createddate));
  emailaddressLabel.innerText = 'OTP associated Email address:' + response.data.emailaddress;
  otpcodecreateddateLabel.innerText = 'OTP created at:' + response.data.otp_createddate + ' and expires:' + timeDifference(new Date(), new Date(response.data.otp_createddate))
  //startTimer(60, 'time');
}


function GenerateOTP() {
  const otpEmailTextbox = document.getElementById('add-otp');

  var subdomainemail = /^\w+([-+.']\w+)*@\w+([-.]{0,0}\w+)\/*.dso.org.sg$/;
  var domainemail = /^\w+([-+.']\w+)*@dso.org.sg$/;

  if (subdomainemail.test(otpEmailTextbox.value.trim()) || domainemail.test(otpEmailTextbox.value.trim())) {

    const item = {
      email: otpEmailTextbox.value.trim()
    };

    fetch(generateotpuri, {
      method: 'POST',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(item)
    })
      .then(response => response.json())
      .then(data => CheckError(data) ? validated_otperror(data) :
        displaygeneratedotp(data, otpEmailTextbox.value.trim()));


  }
  else {
    document.getElementById('errorresponse').style.visibility = 'visible'
    const otperrorLabel = document.getElementById('otperror');
    otperrorLabel.innerText = 'Error: **Invalid Email address';

    const otpcodeLabel = document.getElementById('otpcode');
    const emailsentstatusLabel = document.getElementById('emailsentstatus');

    otpcodeLabel.innerText = ''
    emailsentstatusLabel.innerText = '';
    return false;
  }
}

function ValidateOTP() {
  const otpTextbox = document.getElementById('validate-otp');
  const emailTextbox = document.getElementById('validate-email');
  if (emailTextbox.value.trim() == '' || otpTextbox.value.trim() == '')
    return false;
  const item = {
    otpcode: otpTextbox.value.trim(),
    email: emailTextbox.value.trim()
  };

  fetch(validateotpuri, {
    method: 'POST',
    headers: {
      'Accept': 'application/json',
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(item)
  })
    .then(response => response.json())
    .then(data => CheckError(data) ? generated_otperror(data) :
      displayvalidatedotp(data));
}

function CheckError(response) {
  if (response.status.statusCode == 200) {
    return false;
  } else {
    return true;
  }
}

function startTimer(duration, display) {
  var timer = duration, minutes, seconds;
  const displayTextbox = document.getElementById(display);
  setInterval(function () {
    minutes = parseInt(timer / 60, 10);
    seconds = parseInt(timer % 60, 10);

    minutes = minutes < 10 ? "0" + minutes : minutes;
    seconds = seconds < 10 ? "0" + seconds : seconds;

    displayTextbox.textContent = minutes + ":" + seconds;

    if (--timer < 0) {
      timer = duration;
    }
  }, 1000);
}

function timeDifference(date1, date2) {
  var difference = date1.getTime() - date2.getTime();

  var secondsDifference = Math.floor(difference / 1000);

  console.log('difference = ' +
    secondsDifference + ' second/s ');
  return secondsDifference;
}