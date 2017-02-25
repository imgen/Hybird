declare var Native: (action: string, data: any) => void;

module Hybrid {
    export var lastCallId = 0;
    function executeNativeCommand (commandId: JsToNativeCommandId, callId: number, data: any = null)  {
        Native(JsToNativeCommandId[commandId], {
            callId: callId,
            data: data
        });
    }
    function setNativeToJsCallResult (callId: number, result: CallResult) {
        Native("SetNativeToJsCallResult", {
            CallId: callId,
            Result: result
        });
    }

    function setNativeToJsCallId(callId) {
        Native("SetNativeToJsCallId", callId);
    }

    export enum JsToNativeCommandId {
        ScanQRCode = 1,
        UploadPhoto
    }

    export enum NativeToJsCommandId {
		ShowToast = 1
	}

    interface CallResult {
        status: "success" | "fail" | "error";
        data?: any;
        message?: string;
        code?: number;
    }

    interface CallResultHandler {
        (result: CallResult): void;
    }

    interface CommandHandler {
        (callId: number, data: any): void;
    }

    var callResultHandlerMap: { [key:number]:CallResultHandler; } = {};

    export function setJsToNativeCallResult(callId: number, result: CallResult) {
        var handler = callResultHandlerMap[callId];
        if (handler) {
            delete callResultHandlerMap[callId];
            handler(result);
        }
    }

    var commandHandlerMap: { [key:number]:CommandHandler; } = [];

    export function registerNativeToJsCommandHandler(commandId: NativeToJsCommandId, handler: CommandHandler) {
        commandHandlerMap[commandId] = handler;
    }
    
    export function executeNativeToJsCommand(commandId: NativeToJsCommandId, data: any = null) {
        lastCallId++;
        setNativeToJsCallId(lastCallId);
        var handler = commandHandlerMap[commandId];

        if (handler) {
            handler(lastCallId, data);
        } else {
            setNativeToJsCallResult(lastCallId, {
                status: "fail",
                message: "Invalid command ID"
            });
        }
    }

    export function executeJsToNativeCommand(commandId: JsToNativeCommandId, handler: CallResultHandler = null, data: any = {}) {
        lastCallId++;
        if (handler) {
            callResultHandlerMap[lastCallId] = handler;
        }
        executeNativeCommand(commandId, lastCallId, data);
    }

    export interface ErrorData{
        [key:string]: {} | string;
    }

    export function setSuccessNativeToJsCallResult(callId: number, data: any = null) {
        setNativeToJsCallResult(callId, {
            status: "success",
            data: data
        });
    }

    export function setErrorNativeToJsCallResult(callId: number, message: string, code: number = null, data: ErrorData = null) {
        setNativeToJsCallResult(callId, {
            status: "error",
            message: message,
            code: code,
            data: data
        });
    }

    export function setFailNativeToJsCallResult(callId: number, data: ErrorData) {
        setNativeToJsCallResult(callId, {
            status: "fail",
            data: data
        });
    }
}

window["setJsToNativeCallResult"] = Hybrid.setJsToNativeCallResult;
window["executeNativeToJsCommand"] = Hybrid.executeNativeToJsCommand;

function setSuccessJsToNativeCallResult(callId: number, data: any = null) {
    Hybrid.setJsToNativeCallResult(callId, {
        status: "success",
        data: data
    });
}

function setErrorJsToNativeCallResult(callId: number, message: string, code: number = null, data: Hybrid.ErrorData = null) {
    Hybrid.setJsToNativeCallResult(callId, {
        status: "success",
        message: message,
        data: data,
        code: code
    });
}

function setFailJsToNativeCallResult(callId: number, data: Hybrid.ErrorData) {
    Hybrid.setJsToNativeCallResult(callId, {
        status: "success",
        data: data
    });
}