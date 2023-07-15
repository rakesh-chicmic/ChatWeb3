async function web3Login() {
    baseUrl = "https://localhost:7218"
    if (!window.ethereum) {
        alert("MetaMask not detected. Please install MetaMask first.");
        return;
    }

    const provider = new ethers.providers.Web3Provider(window.ethereum);
    await provider.send("eth_requestAccounts", []);
    const address = await provider.getSigner().getAddress();

    let response = await fetch(
        `${baseUrl}/api/v1/getMessage?address=${address}`
    );

    const temp = await response.json();
    const message = temp.data.message;
    console.log(message);
    const signature = await provider.getSigner().signMessage(message);

    const prefix = "\x19Ethereum Signed Message:\n" + message.length.toString();
    const prefixedMessage = prefix + message;
    const hash = Web3.utils.sha3(prefixedMessage);
    const hashHex = "0x" + hash.slice(2);

    console.log(hashHex);

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

    console.log(data);
    return response;
}