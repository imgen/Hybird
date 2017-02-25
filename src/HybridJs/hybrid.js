var Hybrid;
(function (Hybrid) {
    Hybrid.lastCallId = 0;
    function executeNativeCommand(commandId, callId, data) {
        if (data === void 0) { data = null; }
        Native(JsToNativeCommandId[commandId], {
            callId: callId,
            data: data
        });
    }
    function setNativeToJsCallResult(callId, result) {
        Native("SetNativeToJsCallResult", {
            CallId: callId,
            Result: result
        });
    }
    function setNativeToJsCallId(callId) {
        Native("SetNativeToJsCallId", callId);
    }
    var JsToNativeCommandId;
    (function (JsToNativeCommandId) {
        JsToNativeCommandId[JsToNativeCommandId["ScanQRCode"] = 1] = "ScanQRCode";
        JsToNativeCommandId[JsToNativeCommandId["UploadPhoto"] = 2] = "UploadPhoto";
    })(JsToNativeCommandId = Hybrid.JsToNativeCommandId || (Hybrid.JsToNativeCommandId = {}));
    var NativeToJsCommandId;
    (function (NativeToJsCommandId) {
        NativeToJsCommandId[NativeToJsCommandId["ShowToast"] = 1] = "ShowToast";
    })(NativeToJsCommandId = Hybrid.NativeToJsCommandId || (Hybrid.NativeToJsCommandId = {}));
    var callResultHandlerMap = {};
    function setJsToNativeCallResult(callId, result) {
        var handler = callResultHandlerMap[callId];
        if (handler) {
            delete callResultHandlerMap[callId];
            handler(result);
        }
    }
    Hybrid.setJsToNativeCallResult = setJsToNativeCallResult;
    var commandHandlerMap = [];
    function registerNativeToJsCommandHandler(commandId, handler) {
        commandHandlerMap[commandId] = handler;
    }
    Hybrid.registerNativeToJsCommandHandler = registerNativeToJsCommandHandler;
    function executeNativeToJsCommand(commandId, data) {
        if (data === void 0) { data = null; }
        Hybrid.lastCallId++;
        setNativeToJsCallId(Hybrid.lastCallId);
        var handler = commandHandlerMap[commandId];
        if (handler) {
            handler(Hybrid.lastCallId, data);
        }
        else {
            setNativeToJsCallResult(Hybrid.lastCallId, {
                status: "fail",
                message: "Invalid command ID"
            });
        }
    }
    Hybrid.executeNativeToJsCommand = executeNativeToJsCommand;
    function executeJsToNativeCommand(commandId, handler, data) {
        if (handler === void 0) { handler = null; }
        if (data === void 0) { data = {}; }
        Hybrid.lastCallId++;
        if (handler) {
            callResultHandlerMap[Hybrid.lastCallId] = handler;
        }
        executeNativeCommand(commandId, Hybrid.lastCallId, data);
    }
    Hybrid.executeJsToNativeCommand = executeJsToNativeCommand;
    function setSuccessNativeToJsCallResult(callId, data) {
        if (data === void 0) { data = null; }
        setNativeToJsCallResult(callId, {
            status: "success",
            data: data
        });
    }
    Hybrid.setSuccessNativeToJsCallResult = setSuccessNativeToJsCallResult;
    function setErrorNativeToJsCallResult(callId, message, code, data) {
        if (code === void 0) { code = null; }
        if (data === void 0) { data = null; }
        setNativeToJsCallResult(callId, {
            status: "error",
            message: message,
            code: code,
            data: data
        });
    }
    Hybrid.setErrorNativeToJsCallResult = setErrorNativeToJsCallResult;
    function setFailNativeToJsCallResult(callId, data) {
        setNativeToJsCallResult(callId, {
            status: "fail",
            data: data
        });
    }
    Hybrid.setFailNativeToJsCallResult = setFailNativeToJsCallResult;
})(Hybrid || (Hybrid = {}));
window["setJsToNativeCallResult"] = Hybrid.setJsToNativeCallResult;
window["executeNativeToJsCommand"] = Hybrid.executeNativeToJsCommand;
function setSuccessJsToNativeCallResult(callId, data) {
    if (data === void 0) { data = null; }
    Hybrid.setJsToNativeCallResult(callId, {
        status: "success",
        data: data
    });
}
function setErrorJsToNativeCallResult(callId, message, code, data) {
    if (code === void 0) { code = null; }
    if (data === void 0) { data = null; }
    Hybrid.setJsToNativeCallResult(callId, {
        status: "success",
        message: message,
        data: data,
        code: code
    });
}
function setFailJsToNativeCallResult(callId, data) {
    Hybrid.setJsToNativeCallResult(callId, {
        status: "success",
        data: data
    });
}
//# sourceMappingURL=hybrid.js.map