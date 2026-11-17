using Microsoft.JSInterop;

namespace POS.Client.Helpers
{
    public static class IJSExtentions
    {
        // Para almacenar el token dentro del localStorage del navegador
        public static ValueTask<Object>SetInLocalStorage(this IJSRuntime js, string key, string content) => js.InvokeAsync<Object>("localStorage.setItem", key, content);
        // Para obtener el token dentro del localStorage
        public static ValueTask<string> GetFromLocalStorage(this IJSRuntime js, string key) => js.InvokeAsync<string>("localStorage.getItem", key);
        // Para eliminar el token dentro del localStorage del navegador
        public static ValueTask<Object> RemoveItem(this IJSRuntime js, string key) => js.InvokeAsync<Object>("localStorage.removeItem", key);
    }
}
