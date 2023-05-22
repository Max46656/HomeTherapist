<?php

namespace App\Models;

use Backpack\CRUD\app\Models\Traits\CrudTrait;
use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;

class Article extends Model
{
    use CrudTrait;
    use HasFactory;

    protected $fillable = [
        "user_id",
    ];

    public function user(): BelongsTo
    {
        return $this->belongsTo(User::class, 'user_id', 'staff_id');
    }
}
