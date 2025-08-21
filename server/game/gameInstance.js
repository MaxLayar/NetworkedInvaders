import { randomUUID } from 'crypto';
import Player from './player.js';
import { config } from '../config.js';
import { wss } from '../server.js';
import { 
    sendWelcome,
    sendSimpleMessage,
    sendJsonObject,
    broadcast
} from '../emitters.js';



const connectedPlayers = new Map();
let countdown = null;
let remainingTime = config.game.defaultRoundTime;

function startCountdown() {
    if (countdown) {
        clearInterval(countdown);
    }
    console.log('Start countdown...');
    remainingTime = config.game.defaultRoundTime;

    countdown = setInterval(() => {
        remainingTime--;

        broadcast(
            wss,
            "server:countdown",
            { remainingTime }
        );
        console.log(`Remaining time...${remainingTime}`);

        if (remainingTime <= 0) {
            clearInterval(countdown);
            countdown = null;

            console.log("Round ended!");

            broadcast(
                wss,
                "server:roundEnded",
                { message: "The round has ended!" }
            );
        }
    }, 1000);
}

function stopCountdown() {
    if (countdown) {
        clearInterval(countdown);
        countdown = null;
    }
}

export function playerConnection(ws) {
    ws.clientId = randomUUID();
    console.log(`A new client has connected ${ws.clientId}!`);
    sendWelcome(ws);
}

export function playerDisconnection(ws) {
    if (connectedPlayers.has(ws.clientId)) {
        const player = connectedPlayers.get(ws.clientId);
        connectedPlayers.delete(ws.clientId);
        console.log(`Player ${player.username} (${ws.clientId}) disconnected.`);

        if (connectedPlayers.size === 0)
            stopCountdown();
    } else {
        console.warn(`Disconnection: clientId ${ws.clientId} not found in connectedPlayers.`);
    }
}

export function createPlayer(ws, eventName, requestId, data) {
    // Check if this ws already created a player
    if (connectedPlayers.has(ws.clientId)) {
        sendSimpleMessage(ws, eventName, requestId, false, "You already have a player created.");
        return;
    }

    // Check if there's no player with the same username
    for (let player of connectedPlayers.values()) {
        //TODO: check if username is null
        if (player.name === data.username) {
            sendSimpleMessage(ws, eventName, requestId, false, `A player with the name "${name}" already exists.`);
            return;
        }
    }

    // Finally, create and add the freshly made player to the list
    const newPlayer = new Player(data.username);
    connectedPlayers.set(ws.clientId, newPlayer);

    console.log(`Player created for client ${ws.clientId} with username: ${data.username}`);
    sendSimpleMessage(ws, eventName, requestId, true, newPlayer.username);
    
    // If it's the first player, start the timer
    if (connectedPlayers.size < 2)
        startCountdown();
}

export function scoreUpdate(ws, eventName, requestId, data) {
    const player = connectedPlayers.get(ws.clientId);
    
    if (!player) {
        sendSimpleMessage(ws, eventName, requestId, false, "Player not found.");
        return;
    }

    if (typeof data.score !== "number") {
        sendSimpleMessage(ws, eventName, requestId, false, `Invalid score data: ${data.score}`);
        return;
    }

    if (data.score > player.highscore) {
        player.highscore = data.score;
        console.log(`Player ${player.username} achieved a new highscore: ${player.highscore}`);
    } else {
        console.log(`Player ${player.username} submitted a score of ${data.score}, highscore remains ${player.highscore}`);
    }

    sendJsonObject(ws, eventName, requestId, {
        username: player.username,
        score: data.score,
        highscore: player.highscore
    });
}

export function startGameplay(ws, eventName, requestId, data)
{
    if (connectedPlayers.size < 2 || countdown === null || remainingTime <= 0)
        startCountdown();
}