import { createPlayer } from './game/gameInstance.js';

export const handlers = {
    'createPlayer': createPlayer,
    //Other handlers
};

export function handleMessage(ws, message) {
    try {
        const { event, data } = JSON.parse(message);

        if (handlers[event]) {
            handlers[event](ws, data);
        } else {
            console.warn(`No handler found for event: ${event}`);
            ws.send(JSON.stringify({ error: `Unknown event: ${event}` }));
        }
    } catch (error) {
        console.error('Failed to parse message or handle event:', error);
        ws.send(JSON.stringify({ error: 'Invalid message format.' }));
    }
}