async function web3Login() {
    class customResponse {
        statusCode = 400
        message =  ""
        data = ""
        success = false
    }
    //baseUrl = "https://localhost:7218"
    baseUrl = "http://192.180.0.192:4545"
    if (!window.ethereum) {
        //alert("MetaMask not detected. Please install MetaMask first.");
        let res = new customResponse();
        res.message = "MetaMask not detected.Please install MetaMask first.";
        return res;
    }

    const provider = new ethers.providers.Web3Provider(window.ethereum);
    let address;
    try {
        await provider.send("eth_requestAccounts", []);
        address = await provider.getSigner().getAddress();
        // Proceed with the signed message here
    } catch (error) {
        if (error.code === 4001) {
            // Handle the case where the user rejects signing the message
            console.clear();
            console.log("User rejected request.");
            //alert("User rejected request.");
            let res = new customResponse();
            res.message = "User rejected request";
            return res;
        } else {
            // Handle other signing errors here
            console.clear();
            console.log("Error signing the message:", error);
            //alert("Error signing the message");
            let res = new customResponse();
            res.message = "Error signing the message";
            return res;
        }
    }

    let response = await fetch(
        `${baseUrl}/api/v1/getMessage?address=${address}`
    );

    const temp = await response.json();
    const message = temp.data.message;
    console.log(message);
    //const signature = await provider.getSigner().signMessage(message);
    let signature;
    try {
        signature = await provider.getSigner().signMessage(message);
        // Proceed with the signed message here
    } catch (error) {
        if (error.code === 4001) {
            // Handle the case where the user rejects signing the message
            console.clear();
            console.log("User rejected signing the message.");
            //alert("User rejected signing the message");
            let res = new customResponse();
            res.message = "User rejected signing the message";
            return res;
        } else {
            // Handle other signing errors here
            console.clear();
            console.error("Error signing the message:", error);
            alert("Error signing the message");
            let res = new customResponse();
            res.message = "Error signing the message";
            return res;
        }
        return ;
    }

    const prefix = "\x19Ethereum Signed Message:\n" + message.length.toString();
    const prefixedMessage = prefix + message;
    const hash = Web3.utils.sha3(prefixedMessage);
    const hashHex = "0x" + hash.slice(2);

    //console.log(hashHex);

    response = await fetch(`${baseUrl}/api/v1/verifySignature`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify({
            signer: address, //address of the ethereum account
            signature: signature,
            message: message,
            hash: hashHex,
            //_token: "{{ csrf_token() }}",
        }),
    });
    const data = await response.text();

    //console.log(data);
    return data;
}

function scrollToBottom() {
    console.log("heyy")
    var myDiv = document.getElementById("myDiv");
    console.log("heyy")
    myDiv.scrollTop = myDiv.scrollHeight - myDiv.clientHeight;
}

// Call the scrollToBottom function when needed
scrollToBottom();