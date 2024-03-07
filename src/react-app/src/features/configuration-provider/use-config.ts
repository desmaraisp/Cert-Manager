import { useContext } from "react";
import { ConfigProviderContext } from "./config-context-provider";


export const useConfig = () => useContext(ConfigProviderContext);
