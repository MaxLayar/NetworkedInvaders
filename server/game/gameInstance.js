import { randomUUID } from 'crypto';
import Player from './player.js';
import { sendWelcome, sendPlayerCreated } from '../emitters.js';

const connectedPlayers = new Map();

export function playerConnection(ws) {
    ws.clientId = randomUUID();
    console.log(`A new client has connected ${ws.clientId}!`);
    sendWelcome(ws);
}

export function playerDisconnection(ws, clientId) {
    if (connectedPlayers.has(clientId)) {
        const player = connectedPlayers.get(clientId);
        connectedPlayers.delete(clientId);
        console.log(`Player ${player.username} (${clientId}) disconnected.`);
    } else {
        console.warn(`Disconnection: clientId ${clientId} not found in connectedPlayers.`);
    }
}

export function createPlayer(ws, requestId, data) {
    // Check if this ws already created a player
    if (connectedPlayers.has(ws.clientId)) {
        sendPlayerCreated(ws, requestId, false, "You already have a player created.");
        return;
    }

    // Check if there's no player with the same username
    for (let player of connectedPlayers.values()) {
        if (player.name === data.username) {
            sendPlayerCreated(ws, requestId, false, `A player with the name "${name}" already exists.`);
            return;
        }
    }

    // Finally, create and add the freshly made player to the list
    const newPlayer = new Player(data.username);
    connectedPlayers.set(ws.clientId, newPlayer);

    console.log(`Player created for client ${ws.clientId} with username: ${data.username}`);
    sendPlayerCreated(ws, requestId, true, newPlayer.username);
}