import React from "react";
import {useParams } from "react-router-dom";
const ItemComponent = () => {
    const { ttt } = useParams();
  
    // 使用子項目 ID 做其他操作
    // ...
  
    return (
      <div>
        <h1>子項目 ID: {ttt}</h1>
        <h1>hello</h1>
        
      </div>
    );
  };
  export default ItemComponent;