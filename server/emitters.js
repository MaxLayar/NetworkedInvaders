function send(ws, eventName, requestId, data = {}) {
    ws.send(JSON.stringify({ eventName, requestId, data }));
}

export function sendWelcome(ws) {
    send(ws, "ws:open", "0", {
        clientID: ws.clientId,
        message: "Welcome to the game!"
    });
}

export function sendSimpleMessage(ws, eventName, requestId, success, message) {
    send(ws, eventName, requestId, {
        success: success,
        message: message // can be player.username or error message
    });
}

export function sendJsonObject(ws, eventName, requestId, data = {}) {
    send(ws, eventName, requestId, data);
}

export function broadcast(wss, eventName, data = {}) {
    wss.clients.forEach(client => {
        if (client.readyState === client.OPEN) {
            send(client, eventName, "0", data);
        }
    });
}