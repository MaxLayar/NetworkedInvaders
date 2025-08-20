import { randomUUID } from 'crypto';
import Player from './player.js';
import { sendWelcome, sendError, sendPlayerCreated } from '../emitters.js';

const connectedPlayers = new Map();

export function playerConnection(ws) {
    console.log('A new client has connected.');
    ws.clientID = randomUUID();
    sendWelcome(ws);
}

export function playerDisconnection(ws) {
    if (connectedPlayers.has(ws.clientId)) {
        const player = connectedPlayers.get(ws.clientId);
        connectedPlayers.delete(ws.clientId);
        console.log(`Player ${player.name} (${ws.clientId}) disconnected.`);
    } else {
        console.warn(`Disconnection: clientId ${ws.clientId} not found in connectedPlayers.`);
    }
}

export function createPlayer(ws, name) {
    // Check if this ws already created a player
    if (connectedPlayers.has(ws.clientId)) {
        sendError(ws, "You already have a player created.");
        return;
    }

    // Check if there's no player with the same nickname
    for (let player of connectedPlayers.values()) {
        if (player.name === name) {
            sendError(ws, `A player with the name "${name}" already exists.`);
            return;
        }
    }

    // Finally, create and add the freshly made player to the list
    const newPlayer = new Player(ws.clientId, name);
    connectedPlayers.set(ws.clientId, newPlayer);

    console.log(`Player created: ${name} (${ws.clientId})`);
    sendPlayerCreated(ws, newPlayer);
}